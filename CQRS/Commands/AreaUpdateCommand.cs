namespace Sc3S.CQRS.Commands;

public class AreaUpdateCommand : BaseCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public int AreaId { get; set; }
}