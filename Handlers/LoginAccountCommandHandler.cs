using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3S.Commands;
using Sc3S.Data;
using Sc3S.DTO;

namespace Sc3S.Handlers;

public class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, UserSession>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<LoginAccountCommandHandler> _logger;

    public LoginAccountCommandHandler(IDbContextFactory<Sc3SContext> factory, ILogger<LoginAccountCommandHandler> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public Task<UserSession> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
