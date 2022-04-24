using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class AreaUpdateCommandValidator : AbstractValidator<AreaUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public AreaUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.AreaId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => new { x.Name, x.PlantId, x.AreaId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Areas.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.PlantId==x.PlantId&& a.AreaId!=x.AreaId, CancellationToken);
            return duplicate == null;
        });
        RuleFor(x => x.PlantId).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Plants.AsNoTracking().FirstOrDefaultAsync(m => m.PlantId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
