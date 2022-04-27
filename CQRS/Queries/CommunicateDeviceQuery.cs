namespace Sc3S.CQRS.Queries;

public class CommunicateDeviceQuery : BaseQuery
{
    public int DeviceId { get; set; }
    public int CommunicateId { get; set; }
}