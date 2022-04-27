using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class SituationParameterUpdateCommandValidator : AbstractValidator<SituationParameterUpdateCommand>
{
    public SituationParameterUpdateCommandValidator()
    {
        RuleFor(x => x.SituationId).GreaterThan(0);
        RuleFor(x => x.ParameterId).GreaterThan(0);
    }
}
