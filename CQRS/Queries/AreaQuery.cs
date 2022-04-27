namespace Sc3S.CQRS.Queries;

public class AreaQuery : BaseQuery
{
    public int AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public virtual List<SpaceQuery> Spaces { get; set; } = new();
    public virtual List<CommunicateAreaQuery> CommunicateAreas { get; set; } = new();
}