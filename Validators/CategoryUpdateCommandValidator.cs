﻿using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.Data;

namespace Sc3S.Validators;

public class CategoryUpdateCommandValidator : AbstractValidator<CategoryUpdateCommand>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    public CategoryUpdateCommandValidator(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
        CascadeMode = CascadeMode.Stop;
        RuleFor(x => x.Name).NotEmpty().NotNull().Length(3,50);
        RuleFor(x => x.Description).MaximumLength( 200);
        RuleFor(x=>x.CategoryId).GreaterThanOrEqualTo(0);
        RuleFor(x=> new {x.Name, x.CategoryId}).NotEmpty().NotNull().MustAsync(async (x, CancellationToken) =>
        {
            await using var ctx = await _factory.CreateDbContextAsync(CancellationToken);
            var duplicate = await ctx.Categories.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == x.Name.ToLower().Trim() && a.CategoryId != x.CategoryId, CancellationToken);
            return duplicate == null;
        });
    }
}