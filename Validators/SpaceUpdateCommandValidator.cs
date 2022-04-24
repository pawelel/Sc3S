using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class SpaceUpdateCommandValidator : AbstractValidator<SpaceUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public SpaceUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.SpaceId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => new { x.Name, x.AreaId, x.SpaceId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Spaces.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.AreaId == x.AreaId && a.SpaceId != x.SpaceId, CancellationToken);
            return duplicate == null;
        });
        RuleFor(x => x.AreaId).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Areas.AsNoTracking().FirstOrDefaultAsync(m => m.AreaId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
