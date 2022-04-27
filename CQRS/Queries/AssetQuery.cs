using Sc3S.Enumerations;

namespace Sc3S.CQRS.Queries;

public class AssetQuery : BaseQuery
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int ModelId { get; set; }
    public int CoordinateId { get; set; }
    public virtual List<AssetDetailQuery> AssetDetails { get; set; } = new();
    public virtual List<AssetCategoryQuery> AssetCategories { get; set; } = new();
    public virtual List<CommunicateAssetQuery> CommunicateAssets { get; set; } = new();
    public List<AssetSituationQuery> AssetSituations { get; set; } = new();
}