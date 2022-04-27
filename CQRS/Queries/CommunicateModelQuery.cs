namespace Sc3S.CQRS.Queries;

public class CommunicateModelQuery : BaseQuery
{
    public int ModelId { get; set; }
    public int CommunicateId { get; set; }
}