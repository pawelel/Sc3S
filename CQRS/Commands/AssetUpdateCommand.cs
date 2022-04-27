using Sc3S.Enumerations;

namespace Sc3S.CQRS.Commands;

public class AssetUpdateCommand : BaseCommand
{
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int CoordinateId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public int AssetId { get; set; }
}