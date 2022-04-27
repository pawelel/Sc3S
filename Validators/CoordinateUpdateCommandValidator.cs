using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CoordinateUpdateCommandValidator : AbstractValidator<CoordinateUpdateCommand>
{
    public CoordinateUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.CoordinateId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.SpaceId).GreaterThan(0);
    }
}