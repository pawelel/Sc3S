namespace Sc3S.CQRS.Queries;
public class AssetDetailDisplayQuery : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public string Value { get; set; } = string.Empty;
}
