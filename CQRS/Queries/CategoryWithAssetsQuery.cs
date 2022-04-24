namespace Sc3S.CQRS.Queries;
public class CategoryWithAssetsQuery : BaseDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetQuery> Assets { get; set; } = new();
}
