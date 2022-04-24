namespace Sc3S.CQRS.Commands;
public class PlantUpdateCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PlantId { get; set; }
}
