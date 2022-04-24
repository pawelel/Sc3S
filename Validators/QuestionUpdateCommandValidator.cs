using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class QuestionUpdateCommandValidator : AbstractValidator<QuestionUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public QuestionUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3, 50);
        RuleFor(x => x.QuestionId).GreaterThanOrEqualTo(0);
        RuleFor(x => new { x.Name, x.QuestionId }).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Questions.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.QuestionId != x.QuestionId, CancellationToken);
            return duplicate == null;
        });
    }
}
