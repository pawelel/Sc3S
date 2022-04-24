using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class ModelUpdateCommandValidator : AbstractValidator<ModelUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public ModelUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.ModelId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => new { x.Name, x.ModelId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Models.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.ModelId != x.ModelId, CancellationToken);
            return duplicate == null;
        });
        RuleFor(x => x.DeviceId).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var allowed = await ctx.Devices.AsNoTracking().FirstOrDefaultAsync(m => m.DeviceId == x && m.IsDeleted == false, CancellationToken);
            return allowed != null;
        });
    }
}
