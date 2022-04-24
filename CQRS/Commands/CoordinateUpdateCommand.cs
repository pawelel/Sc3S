namespace Sc3S.CQRS.Commands;
public class CoordinateUpdateCommand
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public int SpaceId { get; set; }
    public int CoordinateId { get; set; }
}
