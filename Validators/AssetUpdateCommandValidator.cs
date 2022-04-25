using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class AssetUpdateCommandValidator :AbstractValidator<AssetUpdateCommand>
{

    public AssetUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).Length(3, 12);
        RuleFor(x => x.AssetId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Process).MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CoordinateId).GreaterThan(0);
        RuleFor(x => x.ModelId).GreaterThan(0);
    }
}
