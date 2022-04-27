namespace Sc3S.CQRS.Queries;

public class AssetCategoryDisplayQuery : BaseQuery
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public int CategoryId { get; set; }
}