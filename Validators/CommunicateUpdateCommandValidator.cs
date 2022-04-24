using FluentValidation;
using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class CommunicateUpdateCommandValidator : AbstractValidator<CommunicateUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public CommunicateUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x=>x.CommunicateId).GreaterThanOrEqualTo(0);
        RuleFor(x => new { x.Name, x.CommunicateId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Communicates.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.CommunicateId != x.CommunicateId, CancellationToken);
            return duplicate == null;
        });
    }
}
