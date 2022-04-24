using Sc3S.CQRS.Queries;
using Sc3S.DTO;

namespace Sc3S.CQRS.Commands;
public class AreaUpdateCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public int AreaId { get; set; }
}
