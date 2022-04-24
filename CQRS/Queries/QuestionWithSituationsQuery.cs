namespace Sc3S.CQRS.Queries;
public class QuestionWithSituationsQuery
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationQuery> Situations { get; set; } = new();
}
