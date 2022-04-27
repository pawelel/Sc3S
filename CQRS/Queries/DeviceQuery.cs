namespace Sc3S.CQRS.Queries;

public class DeviceQuery : BaseQuery
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<ModelQuery> Models { get; set; } = new();
    public virtual List<CommunicateDeviceQuery> CommunicateDevices { get; set; } = new();
    public virtual List<DeviceSituationQuery> DeviceSituations { get; set; } = new();
}