using Sc3S.Enumerations;
namespace Sc3S.DTO;
public class AssetUpdateDto
{
    
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int CoordinateId { get; set; }
    public string Description { get; set; } = string.Empty;
}
