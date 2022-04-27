using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateAssetUpdateCommandValidator : AbstractValidator<CommunicateAssetUpdateCommand>
{
    public CommunicateAssetUpdateCommandValidator()
    {
        RuleFor(x => x.AssetId).GreaterThan(0);
        RuleFor(x => x.CommunicateId).GreaterThan(0);
    }
}
