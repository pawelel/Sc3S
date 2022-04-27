using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateModelUpdateCommandValidator :AbstractValidator<CommunicateModelUpdateCommand>
{
    public CommunicateModelUpdateCommandValidator()
    {
        RuleFor(x => x.CommunicateId).GreaterThan(0);
        RuleFor(x => x.ModelId).GreaterThan(0);
    }
}
