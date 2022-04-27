namespace Sc3S.CQRS.Commands;

public class CommunicateAssetUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int AssetId { get; set; }
}