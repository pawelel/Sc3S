using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.Data;
using Sc3S.DTO;

namespace Sc3S.Validators;

public class ModelParameterDtoValidator : AbstractValidator<ModelParameterDto>
{

    public ModelParameterDtoValidator()
    {
        RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.ModelId).GreaterThan(0);
        RuleFor(x => x.ParameterId).GreaterThan(0);
    }
}
