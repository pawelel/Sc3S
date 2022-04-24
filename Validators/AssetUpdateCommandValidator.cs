using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class AssetUpdateCommandValidator :AbstractValidator<AssetUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public AssetUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).Length(3, 12);
        RuleFor(x => x.AssetId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Process).MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => new { x.Name, x.AssetId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Assets.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.AssetId != x.AssetId, CancellationToken);
            return duplicate == null;
        });
        RuleFor(x => x.ModelId).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Models.AsNoTracking().FirstOrDefaultAsync(m=>m.ModelId == x &&m.IsDeleted==false, CancellationToken);
            return allowed != null;
        });
        RuleFor(x=>x.CoordinateId).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Coordinates.AsNoTracking().FirstOrDefaultAsync(m => m.CoordinateId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
