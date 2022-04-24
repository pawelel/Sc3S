using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class AssetSituationDto : BaseDto
{
    public int AssetId { get; set; }
    public int SituationId { get; set; }
}
