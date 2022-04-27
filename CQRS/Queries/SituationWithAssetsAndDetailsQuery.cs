namespace Sc3S.CQRS.Queries;

public class SituationWithAssetsAndDetailsQuery : BaseQuery
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetWithDetailsDisplayQuery> Assets { get; set; } = new();
}