using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class AccountUpdatePasswordCommandValidator : AbstractValidator<AccountUpdatePasswordCommand>
{
    public AccountUpdatePasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
           .Length(8, 23)
           .Matches(@"[A-Z]+")
           .Matches(@"[a-z]+")
           .Matches(@"[0-9]+")
           .Matches(@"[\!\?\*\.]+");
        RuleFor(x => x.NewPassword)
           .Must(BeDifferentFromOldPassword)
           .WithMessage("Nowe hasło nie może być takie samo jak stare");
    }
    bool BeDifferentFromOldPassword(AccountUpdatePasswordCommand command, string password)
    {
        return command.NewPassword != command.OldPassword;
    }
}
