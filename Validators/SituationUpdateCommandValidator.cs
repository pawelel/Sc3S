using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class SituationUpdateCommandValidator : AbstractValidator<SituationUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public SituationUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.SituationId).GreaterThanOrEqualTo(0);
        RuleFor(x => new { x.Name, x.SituationId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Situations.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.SituationId != x.SituationId, CancellationToken);
            return duplicate == null;
        });
    }
}
