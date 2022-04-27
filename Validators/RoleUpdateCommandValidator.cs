using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class RoleUpdateCommandValidator : AbstractValidator<RoleUpdateCommand>

{
    public RoleUpdateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50).WithName("Rola");
    }
}
