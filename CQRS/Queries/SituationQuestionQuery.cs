namespace Sc3S.CQRS.Queries;

public class SituationQuestionQuery : BaseQuery
{
    public int SituationId { get; set; }
    public int QuestionId { get; set; }
}