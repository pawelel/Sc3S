namespace Sc3S.CQRS.Commands;

public class SituationParameterUpdateCommand : BaseCommand
{ public int SituationId { get; set; } public int ParameterId { get; set; } }