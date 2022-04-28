using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class AccountRoleAndNameUpdateCommandValidator : AbstractValidator<AccountRoleAndNameUpdateCommand>
{
    public AccountRoleAndNameUpdateCommandValidator()
    {
        RuleFor(x => x.UserName)
          .Length(4, 13).Matches("^[a-zA-Z0-9]*$");
        RuleFor(x => x.RoleId).NotNull().NotEmpty();
    }
}
