using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class SituationUpdateCommandValidator : AbstractValidator<SituationUpdateCommand>
{
    public SituationUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.SituationId).GreaterThanOrEqualTo(0);
    }
}