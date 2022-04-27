namespace Sc3S.CQRS.Queries;

public class SituationDetailQuery : BaseQuery
{
    public int SituationId { get; set; }
    public int DetailId { get; set; }
}