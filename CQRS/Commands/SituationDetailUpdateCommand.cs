namespace Sc3S.CQRS.Commands;

public class SituationDetailUpdateCommand : BaseCommand
{ public int SituationId { get; set; } public int DetailId { get; set; } }