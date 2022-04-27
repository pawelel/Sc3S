using Sc3S.Enumerations;

namespace Sc3S.CQRS.Commands;

public class SpaceUpdateCommand : BaseCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public SpaceType SpaceType { get; set; }
    public int SpaceId { get; set; }
}