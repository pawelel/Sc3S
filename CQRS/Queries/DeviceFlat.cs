namespace Sc3S.CQRS.Queries;
public class DeviceFlat : BaseDto
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
