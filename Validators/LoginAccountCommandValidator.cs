using FluentValidation;

using Sc3S.Authentication;
using Sc3S.Commands;
using Sc3S.DTO;

namespace Sc3S.Validators;

public class LoginAccountCommandValidator : AbstractValidator<LoginAccountCommand>
{
    public LoginAccountCommandValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithName("Nazwa Użytkownika");
        RuleFor(u => u.Password)
            .NotEmpty()
            .WithName("Hasło");
    }
}
