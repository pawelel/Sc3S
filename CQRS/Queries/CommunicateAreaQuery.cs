namespace Sc3S.CQRS.Queries;

public class CommunicateAreaQuery : BaseQuery
{
    public int AreaId { get; set; }
    public int CommunicateId { get; set; }
}