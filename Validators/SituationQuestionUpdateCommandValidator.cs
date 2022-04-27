using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class SituationQuestionUpdateCommandValidator : AbstractValidator<SituationQuestionUpdateCommand>

{
    public SituationQuestionUpdateCommandValidator()
    {
        RuleFor(x => x.SituationId).GreaterThan(0);
        RuleFor(x => x.QuestionId).GreaterThan(0);
    }
}
