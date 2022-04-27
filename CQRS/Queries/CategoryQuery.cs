namespace Sc3S.CQRS.Queries;

public class CategoryQuery : BaseQuery
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetCategoryQuery> AssetCategories { get; set; } = new();
    public virtual List<CategorySituationQuery> CategorySituations { get; set; } = new();
    public virtual List<CommunicateCategoryQuery> CommunicateCategories { get; set; } = new();
}