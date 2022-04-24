namespace Sc3S.CQRS.Queries;
public class AssetWithSituationsAndCategoriesQuery
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public virtual List<CategoryQuery> Categories { get; set; } = new();
    public virtual List<SituationQuery> Situations { get; set; } = new();
}
