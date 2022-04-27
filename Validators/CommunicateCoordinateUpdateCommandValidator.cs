using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateCoordinateUpdateCommandValidator : AbstractValidator<CommunicateCoordinateUpdateCommand>

{
    public CommunicateCoordinateUpdateCommandValidator()
    {
        RuleFor(x => x.CoordinateId).GreaterThan(0);
        RuleFor(x => x.CommunicateId).GreaterThan(0);
    }
}
