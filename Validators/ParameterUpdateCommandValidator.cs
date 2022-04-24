using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class ParameterUpdateCommandValidator : AbstractValidator<ParameterUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public ParameterUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.ParameterId).GreaterThanOrEqualTo(0);
        RuleFor(x => new { x.Name, x.ParameterId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Parameters.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.ParameterId != x.ParameterId, CancellationToken);
            return duplicate == null;
        });
    }
}
