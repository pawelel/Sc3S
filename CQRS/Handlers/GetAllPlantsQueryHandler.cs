using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;

namespace Sc3S.CQRS.Handlers;

public class GetAllPlantsQueryHandler : IRequestHandler<GetAllPlantsQuery, Plant[]>
{
    private readonly IDbContextFactory<Sc3SContext> _factory;

    public GetAllPlantsQueryHandler(IDbContextFactory<Sc3SContext> factory)
    {
        _factory = factory;
    }

    public async Task<Plant[]> Handle(GetAllPlantsQuery request, CancellationToken cancellationToken)
    {
        await using var ctx = await _factory.CreateDbContextAsync(cancellationToken);
        return await ctx.Plants.ToArrayAsync(cancellationToken: cancellationToken);
    }
}
