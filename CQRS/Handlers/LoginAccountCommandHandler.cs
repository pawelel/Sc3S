using MediatR;

using Microsoft.EntityFrameworkCore;
using System.Linq;
using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;

using System.Security.Claims;

namespace Sc3S.CQRS.Handlers;

public class LoginAccountCommandHandler : IRequestHandler<AccountLoginCommand, UserSession>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<LoginAccountCommandHandler> _logger;

    public LoginAccountCommandHandler(IDbContextFactory<Sc3SContext> factory, ILogger<LoginAccountCommandHandler> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<UserSession> Handle(AccountLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
            var user = await ctx.Accounts.Include(a=>a.Role).AsNoTracking().FirstOrDefaultAsync(a => a.UserName.ToLower() == request.UserName.ToLower() || a.Email.ToLower() == request.UserName.ToLower(), cancellationToken: cancellationToken);
            if (user is not null)
            {
                
                return new()
                {
                    UserName = user.UserName,
                    Role = user.Role.Name
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while logging user");
        }
        return null!;
    }
}