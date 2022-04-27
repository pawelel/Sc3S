namespace Sc3S.CQRS.Queries;

public class SituationParameterQuery : BaseQuery
{
    public int SituationId { get; set; }
    public int ParameterId { get; set; }
}