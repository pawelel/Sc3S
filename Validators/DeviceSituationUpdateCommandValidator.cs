using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class DeviceSituationUpdateCommandValidator : AbstractValidator<DeviceSituationUpdateCommand>

{
    public DeviceSituationUpdateCommandValidator()
    {
        RuleFor(x => x.DeviceId).GreaterThan(0);
        RuleFor(x => x.SituationId).GreaterThan(0);
    }
}
