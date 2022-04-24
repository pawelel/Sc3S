using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CommunicateAssetDto : BaseDto
{
    public int AssetId { get; set; }
    public int CommunicateId { get; set; }
}
