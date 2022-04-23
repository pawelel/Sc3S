using MediatR;

using Sc3S.DTO;
using Sc3S.Entities;

namespace Sc3S.Commands;

public class CreateAccountCommand : IRequest<AccountDto>
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
