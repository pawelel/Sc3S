using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Helpers;

namespace Sc3S.Services;

public interface IAccountService
{
    Task<ServiceResponse> CreateAccount(AccountUpdateCommand command);

    Task<ServiceResponse> UpdateAccount(AccountUpdateCommand command);

    Task<ServiceResponse> DeleteAccount(string userId);

    Task<ServiceResponse<AccountDisplayQuery>> GetAccountById(string userId);

    Task<ServiceResponse<AccountDisplayQuery>> GetAccountByEmail(string email);

    Task<ServiceResponse<AccountDisplayQuery>> GetAccountByUserName(string name);

    Task<ServiceResponse<IEnumerable<AccountDisplayQuery>>> GetAccounts();

    Task<ServiceResponse> UpdateAccountPassword(AccountUpdatePasswordCommand command);

    Task<ServiceResponse> MarkDeleteAccount(string updatedBy, string userId);
}

public class AccountService : IAccountService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IDbContextFactory<Sc3SContext> factory, ILogger<AccountService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResponse> CreateAccount(AccountUpdateCommand command)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var duplicateName = await context.Accounts.AnyAsync(x => x.UserName.ToLower() == command.UserName.ToLower());
        if (duplicateName)
        {
            return new ServiceResponse(false, "Nazwa użytkownika jest już zajęta");
        }
        var duplicateEmail = await context.Accounts.AnyAsync(x => x.Email.ToLower() == command.Email.ToLower());
        if (duplicateEmail)
        {
            return new ServiceResponse(false, "Email jest już zajęty");
        }

        var hasher = new PasswordHasher<Account>();
        var account = new Account
        {
            UserName = command.UserName,
            Email = command.Email,
            PasswordHash = hasher.HashPassword(null!, command.Password),
            RoleId = command.RoleId,
            IsDeleted = false,
            CreatedBy = command.UpdatedBy,
            UpdatedBy = command.UpdatedBy,
        };
        context.Accounts.Add(account);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Account {UserName} created", account.UserName);
            return new ServiceResponse(true, "Konto zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account {UserName}", account.UserName);
            return new ServiceResponse(false, "Wystąpił błąd podczas tworzenia konta");
        }
    }

    public async Task<ServiceResponse<AccountDisplayQuery>> GetAccountById(string userId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts
            .AsNoTracking()
            .Select(x => new AccountDisplayQuery
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
                IsDeleted = x.IsDeleted,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                UpdatedBy = x.UpdatedBy,
                UpdatedOn = x.UpdatedOn
            })
            .FirstOrDefaultAsync(x => x.UserId == userId);
        if (account == null)
        {
            return new ServiceResponse<AccountDisplayQuery>(false, null, "Nie znaleziono konta");
        }
        return new ServiceResponse<AccountDisplayQuery>(true, account, "Konto zostało znalezione");
    }

    public async Task<ServiceResponse<AccountDisplayQuery>> GetAccountByUserName(string name)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts
            .AsNoTracking()
            .Select(x => new AccountDisplayQuery
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
                IsDeleted = x.IsDeleted,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                UpdatedBy = x.UpdatedBy,
                UpdatedOn = x.UpdatedOn
            })
            .FirstOrDefaultAsync(x => x.UserName.ToLower() == name.ToLower());
        if (account == null)
        {
            return new ServiceResponse<AccountDisplayQuery>(false, null, "Nie znaleziono konta");
        }
        return new ServiceResponse<AccountDisplayQuery>(true, account, "Konto zostało znalezione");
    }

    public async Task<ServiceResponse> MarkDeleteAccount(string updatedBy, string userId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);
        if (account == null)
        {
            return new ServiceResponse(false, "Nie znaleziono konta");
        }
        if (account.IsDeleted)
        {
            return new ServiceResponse(false, "Konto zostało już oznaczone jako usunięte");
        }
        account.IsDeleted = true;
        account.UpdatedBy = updatedBy;
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Account {UserName} marked as deleted", account.UserName);
            return new ServiceResponse(true, "Konto zostało oznaczone jako usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking account {UserName} as deleted", account.UserName);
            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania konta jako usunięte");
        }
    }

    public async Task<ServiceResponse> DeleteAccount(string userId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);
        if (account == null)
        {
            return new ServiceResponse(false, "Nie znaleziono konta");
        }
        if (!account.IsDeleted)
        {
            return new ServiceResponse(false, "Konto nie zostało oznaczone jako usunięte");
        }
        context.Accounts.Remove(account);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Account {UserName} deleted", account.UserName);
            return new ServiceResponse(true, "Konto zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account {UserName}", account.UserName);
            return new ServiceResponse(false, "Wystąpił błąd podczas usuwania konta");
        }
    }

    public async Task<ServiceResponse> UpdateAccount(AccountUpdateCommand command)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == command.UserId);
        if (account == null)
        {
            return new ServiceResponse(false, "Nie znaleziono konta");
        }
        if (account.UserName.ToLower() != command.UserName.ToLower())
        {
            var duplicateName = await context.Accounts.AnyAsync(x => x.UserName.ToLower() == command.UserName.ToLower());
            if (duplicateName)
            {
                return new ServiceResponse(false, "Nazwa użytkownika jest już zajęta");
            }
        }
        if (account.Email.ToLower() != command.Email.ToLower())
        {
            var duplicateEmail = await context.Accounts.AnyAsync(x => x.Email.ToLower() == command.Email.ToLower());
            if (duplicateEmail)
            {
                return new ServiceResponse(false, "Email jest już zajęty");
            }
        }
        account.UserName = command.UserName;
        account.Email = command.Email;
        account.UpdatedBy = command.UpdatedBy;
        if (command.Password != null)
        {
            var hasher = new PasswordHasher<Account>();
            account.PasswordHash = hasher.HashPassword(null!, command.Password);
        }
        if (account.RoleId != command.RoleId)
        {
            var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.RoleId == command.RoleId);
            if (role == null)
            {
                return new ServiceResponse(false, "Nie znaleziono roli");
            }
            account.RoleId = command.RoleId;
        }
        account.IsDeleted = false;
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Account {UserName} updated", account.UserName);
            return new ServiceResponse(true, "Konto zostało zaktualizowane");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {UserName}", account.UserName);
            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji konta");
        }
    }

    public async Task<ServiceResponse> UpdateAccountPassword(AccountUpdatePasswordCommand command)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts.FirstOrDefaultAsync(x => x.UserId == command.UserId);
        if (account == null)
        {
            return new ServiceResponse(false, "Nie znaleziono konta");
        }
        var hasher = new PasswordHasher<Account>();
        if (hasher.VerifyHashedPassword(null!, account.PasswordHash, command.OldPassword) != PasswordVerificationResult.Success)
        {
            return new ServiceResponse(false, "Stare hasło jest niepoprawne");
        }
        account.PasswordHash = hasher.HashPassword(null!, command.NewPassword);
        account.UpdatedBy = command.UpdatedBy;
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Account {UserName} password updated", account.UserName);
            return new ServiceResponse(true, "Hasło konta zostało zaktualizowane");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {UserName} password", account.UserName);
            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji hasła konta");
        }
    }

    public async Task<ServiceResponse<AccountDisplayQuery>> GetAccountByEmail(string email)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var account = await context.Accounts
            .AsNoTracking()
            .Select(x => new AccountDisplayQuery
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
                IsDeleted = x.IsDeleted,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                UpdatedBy = x.UpdatedBy,
                UpdatedOn = x.UpdatedOn
            })
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        if (account == null)
        {
            return new ServiceResponse<AccountDisplayQuery>(false, null, "Nie znaleziono konta");
        }
        return new ServiceResponse<AccountDisplayQuery>(true, account, "Konto zostało znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<AccountDisplayQuery>>> GetAccounts()
    {
        await using var context = await _factory.CreateDbContextAsync();
        var accounts = await context.Accounts
            .AsNoTracking()
            .Select(x => new AccountDisplayQuery
            {
                UserId = x.UserId,
                UserName = x.UserName,
                Email = x.Email,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
                IsDeleted = x.IsDeleted,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                UpdatedBy = x.UpdatedBy,
                UpdatedOn = x.UpdatedOn
            })
            .ToListAsync();
        if (accounts == null)
        {
            return new ServiceResponse<IEnumerable<AccountDisplayQuery>>(false, null, "Nie znaleziono kont");
        }
        return new ServiceResponse<IEnumerable<AccountDisplayQuery>>(true, accounts, "Konta zostały znalezione");
    }
}