namespace Sc3S.CQRS.Queries;

public class DeviceFlat : BaseQuery
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}