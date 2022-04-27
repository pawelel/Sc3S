namespace Sc3S.CQRS.Commands;

public class DeviceSituationUpdateCommand : BaseCommand
{ public int DeviceId { get; set; } public int SituationId { get; set; } }