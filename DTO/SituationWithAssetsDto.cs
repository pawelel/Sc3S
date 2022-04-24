namespace Sc3S.DTO;
public class SituationWithAssetsDto : BaseDto
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetDto> Assets { get; set; } = new();
}
