namespace Sc3S.CQRS.Queries;

public class CommunicateCoordinateQuery : BaseQuery
{
    public int CoordinateId { get; set; }
    public int CommunicateId { get; set; }
}