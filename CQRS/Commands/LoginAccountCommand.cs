using MediatR;

using Sc3S.CQRS.Queries;

namespace Sc3S.CQRS.Commands
{
    public class LoginAccountCommand : IRequest<UserSession>
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}