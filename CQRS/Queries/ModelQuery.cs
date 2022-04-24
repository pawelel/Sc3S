using Sc3S.DTO;

namespace Sc3S.CQRS.Queries;
public class ModelQuery : BaseDto
{
    public int ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<ModelParameterDto> ModelParameters { get; set; } = new();
    public virtual List<AssetQuery> Assets { get; set; } = new();
}
