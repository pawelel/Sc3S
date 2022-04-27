using FluentValidation;

using Sc3S.CQRS.Commands;

namespace Sc3S.Validators;

public class ModelParameterUpdateCommandValidator : AbstractValidator<ModelParameterUpdateCommand>
{
    public ModelParameterUpdateCommandValidator()
    {
        RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.ModelId).GreaterThan(0);
        RuleFor(x => x.ParameterId).GreaterThan(0);
    }
}