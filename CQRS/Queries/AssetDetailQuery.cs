namespace Sc3S.CQRS.Queries;

public class AssetDetailQuery : BaseQuery
{
    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public string Value { get; set; } = string.Empty;
}