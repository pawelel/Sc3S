namespace Sc3S.CQRS.Queries;

public class DeviceSituationQuery : BaseQuery
{
    public int SituationId { get; set; }
    public int DeviceId { get; set; }
}