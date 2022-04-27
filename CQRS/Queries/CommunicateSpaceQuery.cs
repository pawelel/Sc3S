namespace Sc3S.CQRS.Queries;

public class CommunicateSpaceQuery : BaseQuery
{
    public int CommunicateId { get; set; }
    public int SpaceId { get; set; }
}