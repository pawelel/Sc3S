namespace Sc3S.CQRS.Queries;
public class SituationWithQuestionsQuery : BaseDto
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<QuestionQuery> Questions { get; set; } = new();
}
