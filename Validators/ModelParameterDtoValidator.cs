using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.Data;
using Sc3S.DTO;

namespace Sc3S.Validators;

public class ModelParameterDtoValidator : AbstractValidator<ModelParameterDto>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public ModelParameterDtoValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.ModelId).GreaterThan(0).MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Models.AsNoTracking().FirstOrDefaultAsync(m => m.ModelId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
        RuleFor(x => x.ParameterId).GreaterThan(0).MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Parameters.AsNoTracking().FirstOrDefaultAsync(m => m.ParameterId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
