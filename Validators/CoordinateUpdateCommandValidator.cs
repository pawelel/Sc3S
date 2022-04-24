using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class CoordinateUpdateCommandValidator :AbstractValidator<CoordinateUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public CoordinateUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x=>x.CoordinateId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => new { x.Name, x.CoordinateId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Coordinates.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.CoordinateId != x.CoordinateId, CancellationToken);
            return duplicate == null;
        });
        RuleFor(x => x.SpaceId).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Spaces.AsNoTracking().FirstOrDefaultAsync(m => m.SpaceId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
