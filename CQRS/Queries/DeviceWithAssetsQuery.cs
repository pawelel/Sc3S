namespace Sc3S.CQRS.Queries;

public class DeviceWithAssetsQuery : BaseQuery
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetQuery> Assets { get; set; } = new();
}