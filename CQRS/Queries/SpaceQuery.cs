using Sc3S.DTO;
using Sc3S.Enumerations;
namespace Sc3S.CQRS.Queries;
public class SpaceQuery : BaseDto
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public SpaceType SpaceType { get; set; }
    public virtual List<CoordinateQuery> Coordinates { get; set; } = new();

    public virtual List<CommunicateSpaceDto> CommunicateSpaces { get; set; } = new();
}
