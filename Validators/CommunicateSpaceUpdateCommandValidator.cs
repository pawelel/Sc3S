using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateSpaceUpdateCommandValidator : AbstractValidator<CommunicateSpaceUpdateCommand>

{
    public CommunicateSpaceUpdateCommandValidator()
    {
        RuleFor(x => x.CommunicateId).GreaterThan(0);
        RuleFor(x => x.SpaceId).GreaterThan(0);
    }
}
