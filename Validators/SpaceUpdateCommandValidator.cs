using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class SpaceUpdateCommandValidator : AbstractValidator<SpaceUpdateCommand>
{

    public SpaceUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.SpaceId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.AreaId).GreaterThan(0);
        RuleFor(x => x.SpaceType).IsInEnum();
    }
}
