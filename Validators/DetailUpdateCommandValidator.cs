using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class DetailUpdateCommandValidator : AbstractValidator<DetailUpdateCommand>
{
    public DetailUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.DetailId).GreaterThanOrEqualTo(0);
    }
}