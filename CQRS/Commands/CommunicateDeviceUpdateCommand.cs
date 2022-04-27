namespace Sc3S.CQRS.Commands;

public class CommunicateDeviceUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int DeviceId { get; set; }
}