using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;
using Sc3S.Entities;

namespace Sc3S.Validators;

public class AccountLoginCommandValidator : AbstractValidator<AccountLoginCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly IPasswordHasher<Account> _hasher;

    public AccountLoginCommandValidator(IDbContextFactory<Sc3SContext> factory, IPasswordHasher<Account> hasher)
    {
        CascadeMode = CascadeMode.Stop;
        _factory = factory;
        _hasher = hasher;
        RuleFor(u => u.UserName)
            .NotEmpty().NotNull()
            .WithName("Nazwa Użytkownika")
            .Length(5, 50);
        RuleFor(u => u.Password)
            .NotEmpty().NotNull()
            .WithName("Hasło")
            .Length(8, 23);
        RuleFor(x => new { x.UserName, x.Password }).MustAsync(async (x, CancellationToken) =>
          {
              await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
              var user = await ctx.Accounts.AsNoTracking().FirstOrDefaultAsync(y => y.UserName.ToLower() == x.UserName.ToLower() || y.Email.ToLower() == x.UserName.ToLower(), cancellationToken: CancellationToken);
              if (user is not null)
              {
                  var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, x.Password);
                  return result == PasswordVerificationResult.Success;
              }
              return false;
          }).WithMessage("Nazwa użytkownika lub hasło niepoprawne");
    }
}