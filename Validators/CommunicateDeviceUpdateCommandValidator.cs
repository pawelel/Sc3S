using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateDeviceUpdateCommandValidator : AbstractValidator<CommunicateDeviceUpdateCommand>

{
    public CommunicateDeviceUpdateCommandValidator()
    {
        RuleFor(x => x.DeviceId).GreaterThan(0);
        RuleFor(x => x.CommunicateId).GreaterThan(0);
    }
}
