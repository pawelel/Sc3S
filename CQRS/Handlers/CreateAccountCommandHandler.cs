using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;
using Sc3S.DTO;
using Sc3S.Entities;

namespace Sc3S.CQRS.Handlers;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<CreateAccountCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public CreateAccountCommandHandler(IDbContextFactory<Sc3SContext> factory, ILogger<CreateAccountCommandHandler> logger, IMapper mapper, IPasswordHasher<Account> passwordHasher)
    {
        _factory = factory;
        _logger = logger;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
            Account account = new()
            {
                UserName = request.UserName,
                Email = request.Email
            };
            await Task.FromResult(account.PasswordHash = _passwordHasher.HashPassword(account, request.Password));

            await ctx.AddAsync(account, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AccountDto>(account);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error while creating user account");
            return null!;
        }
        
    }
}
