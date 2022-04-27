namespace Sc3S.CQRS.Queries;

public class CategorySituationQuery : BaseQuery
{
    public int CategoryId { get; set; }
    public int SituationId { get; set; }
}