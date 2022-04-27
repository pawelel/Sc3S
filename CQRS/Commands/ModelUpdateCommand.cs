namespace Sc3S.CQRS.Commands;

public class ModelUpdateCommand : BaseCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DeviceId { get; set; }
    public int ModelId { get; set; }
}