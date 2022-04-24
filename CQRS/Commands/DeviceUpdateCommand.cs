namespace Sc3S.CQRS.Commands;
public class DeviceUpdateCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DeviceId { get; set; }
}
