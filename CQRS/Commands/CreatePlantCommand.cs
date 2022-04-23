using MediatR;

using Sc3S.Entities;

namespace Sc3S.CQRS.Commands;

public class CreatePlantCommand : IRequest<Plant>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
