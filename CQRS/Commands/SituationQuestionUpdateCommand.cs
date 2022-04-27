namespace Sc3S.CQRS.Commands;

public class SituationQuestionUpdateCommand : BaseCommand
{
    public int QuestionId { get; set; }
    public int SituationId { get; set; }
}