namespace Sc3S.DTO;
public class ModelDto : BaseDto
{
    public int ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<ModelParameterDto> ModelParameters { get; set; } = new();
    public virtual List<AssetDto> Assets { get; set; } = new();
}
