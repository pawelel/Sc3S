namespace Sc3S.CQRS.Commands;

public class AssetSituationUpdateCommand : BaseCommand
{
    public int AssetId { get; set; }
    public int SituationId { get; set; }
}
