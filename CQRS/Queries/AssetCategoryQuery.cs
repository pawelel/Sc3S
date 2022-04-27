namespace Sc3S.CQRS.Queries;

public class AssetCategoryQuery : BaseQuery
{
    public int AssetId { get; set; }
    public int CategoryId { get; set; }
}