using Sc3S.DTO;

namespace Sc3S.CQRS.Queries;
public class CoordinateQuery : BaseDto
{
    public int CoordinateId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public virtual List<CommunicateCoordinateDto> CommunicateCoordinates { get; set; } = new();
    public virtual List<AssetQuery> Assets { get; set; } = new();
}
