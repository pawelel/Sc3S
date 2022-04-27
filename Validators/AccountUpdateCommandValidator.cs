using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class AccountUpdateCommandValidator : AbstractValidator<AccountUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public AccountUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        CascadeMode = CascadeMode.Stop;
        _factory = factory;

        RuleFor(x => x.UserName).MustAsync(async (x, CancellationToken) =>
          {
              await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
              var duplicate = await ctx.Accounts.AnyAsync(y => Equals(y.UserName.ToLower(), x.ToLower()), cancellationToken: CancellationToken);
              return !duplicate;
          }).WithMessage("Ta nazwa jest już zajęta")
          .Length(4, 13).Matches("^[a-zA-Z0-9]*$");

        RuleFor(x => x.Email).MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Accounts.AnyAsync(y => Equals(y.Email.ToLower(), x.ToLower()), cancellationToken: CancellationToken);
            return !duplicate;
        }).WithMessage("Ten Email jest już zajęty").EmailAddress().Length(14, 50);
        RuleFor(x => x.Password)
            .Length(8, 23)
            .Matches(@"[A-Z]+")
            .Matches(@"[a-z]+")
            .Matches(@"[0-9]+")
            .Matches(@"[\!\?\*\.]+");
    }
}