using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.Data;
using Sc3S.DTO;

namespace Sc3S.Validators;

public class AssetDetailDtoValidator : AbstractValidator<AssetDetailDto>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public AssetDetailDtoValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        RuleFor(x => x.Value).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x=>x.AssetId).GreaterThan(0).MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Assets.AsNoTracking().FirstOrDefaultAsync(m => m.AssetId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
        RuleFor(x=>x.DetailId).GreaterThan(0).MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Details.AsNoTracking().FirstOrDefaultAsync(m => m.DetailId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
