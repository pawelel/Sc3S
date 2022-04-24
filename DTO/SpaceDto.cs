using Sc3S.Enumerations;
namespace Sc3S.DTO;
public class SpaceDto : BaseDto
{
    public int SpaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public virtual List<CoordinateDto> Coordinates { get; set; } = new();
    public SpaceType SpaceType { get; set; }

    public virtual List<CommunicateSpaceDto> CommunicateSpaces { get; set; } = new();
}
