namespace Sc3S.CQRS.Queries;

public class CoordinateQuery : BaseQuery
{
    public int CoordinateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SpaceId { get; set; }

    public string Description { get; set; } = string.Empty;

    public virtual List<CommunicateCoordinateQuery> CommunicateCoordinates { get; set; } = new();
    public virtual List<AssetQuery> Assets { get; set; } = new();
}