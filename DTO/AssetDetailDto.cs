using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class AssetDetailDto : BaseDto
{
    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public string Value { get; set; } = string.Empty;
}
