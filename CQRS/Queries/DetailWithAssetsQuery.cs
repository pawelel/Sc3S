namespace Sc3S.CQRS.Queries;
public class DetailWithAssetsQuery : BaseDto
{
    public int DetailId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public virtual List<AssetQuery> Assets { get; set; } = new();
}
