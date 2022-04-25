using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.Data;
using Sc3S.DTO;

namespace Sc3S.Validators;

public class AssetDetailDtoValidator : AbstractValidator<AssetDetailDto>
{

    public AssetDetailDtoValidator()
    {
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssetId).GreaterThan(0);
        RuleFor(x => x.DetailId).GreaterThan(0);
    }
}
