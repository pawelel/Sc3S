using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CommunicateDeviceDto : BaseDto
{
    public int DeviceId { get; set; }
    public int CommunicateId { get; set; }
}
