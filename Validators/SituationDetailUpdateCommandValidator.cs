using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class SituationDetailUpdateCommandValidator : AbstractValidator<SituationDetailUpdateCommand>

{
    public SituationDetailUpdateCommandValidator()
    {
        RuleFor(x => x.SituationId).GreaterThan(0);

        RuleFor(x => x.DetailId).GreaterThan(0);
    }
}
