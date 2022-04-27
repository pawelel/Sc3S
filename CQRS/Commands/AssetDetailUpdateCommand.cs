namespace Sc3S.CQRS.Commands;

public class AssetDetailUpdateCommand : BaseCommand
{
    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public string Value { get; set; } = string.Empty;
}
