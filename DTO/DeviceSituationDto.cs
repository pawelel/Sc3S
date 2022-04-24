using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class DeviceSituationDto : BaseDto
{
    public int SituationId { get; set; }
    public int DeviceId { get; set; }
}
