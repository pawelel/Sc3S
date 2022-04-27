namespace Sc3S.CQRS.Queries;

/// <summary>
///     Trouble shooting question not from DB
/// </summary>
public class QuestionQuery : BaseQuery
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationQuestionQuery> SituationQuestions { get; set; } = new();
}