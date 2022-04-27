namespace Sc3S.CQRS.Queries;

public class AssetSituationQuery : BaseQuery
{
    public int AssetId { get; set; }
    public int SituationId { get; set; }
}