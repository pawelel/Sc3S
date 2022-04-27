using FluentValidation;
using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class CommunicateCategoryUpdateCommandValidator : AbstractValidator<CommunicateCategoryUpdateCommand>

{
    public CommunicateCategoryUpdateCommandValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.CommunicateId).GreaterThan(0);
    }
}
