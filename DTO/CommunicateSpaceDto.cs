using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CommunicateSpaceDto : BaseDto
{

    public int CommunicateId { get; set; }
    public int SpaceId { get; set; }
}
