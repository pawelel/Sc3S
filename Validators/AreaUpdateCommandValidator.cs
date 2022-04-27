using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class AreaUpdateCommandValidator : AbstractValidator<AreaUpdateCommand>
{
    public AreaUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.AreaId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.PlantId).GreaterThan(0);
    }
}