using MediatR;

using Sc3S.Entities;

namespace Sc3S.CQRS.Queries;

public class GetAllPlantsQuery : IRequest<Plant[]>
{
}
