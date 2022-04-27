using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class AssetDetailUpdateCommandValidator : AbstractValidator<AssetDetailUpdateCommand>
{
    public AssetDetailUpdateCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssetId).GreaterThan(0);
        RuleFor(x => x.DetailId).GreaterThan(0);
    }
}