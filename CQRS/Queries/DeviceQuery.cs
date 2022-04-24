using Sc3S.DTO;

namespace Sc3S.CQRS.Queries;
public class DeviceQuery : BaseDto
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<ModelQuery> Models { get; set; } = new();
    public virtual List<CommunicateDeviceDto> CommunicateDevices { get; set; } = new();
    public virtual List<DeviceSituationDto> DeviceSituations { get; set; } = new();
}
