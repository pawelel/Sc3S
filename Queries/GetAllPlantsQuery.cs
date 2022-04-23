using MediatR;

using Sc3S.Entities;

namespace Sc3S.Queries;

public class GetAllPlantsQuery : IRequest<Plant[]>
{
}
