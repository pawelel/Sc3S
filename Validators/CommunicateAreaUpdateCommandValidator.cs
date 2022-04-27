using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateAreaUpdateCommandValidator : AbstractValidator<CommunicateAreaUpdateCommand>
{
    public CommunicateAreaUpdateCommandValidator()
    {
        RuleFor(x => x.CommunicateId).GreaterThan(0);
        RuleFor(x => x.AreaId).GreaterThan(0);
    }
}
