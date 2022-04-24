namespace Sc3S.CQRS.Queries;
public class PlantQuery : BaseDto
{
    public int PlantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AreaQuery> Areas { get; set; } = new();
}
