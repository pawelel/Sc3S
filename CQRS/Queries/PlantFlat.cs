namespace Sc3S.CQRS.Queries;

public class PlantFlat : BaseQuery
{
    public int PlantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}