namespace Sc3S.Entities;

public class Plant : BaseEntity
{
    public int PlantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<Area> Areas { get; set; } = new List<Area>();
}
