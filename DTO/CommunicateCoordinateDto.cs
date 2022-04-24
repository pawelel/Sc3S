using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CommunicateCoordinateDto : BaseDto
{
    public int CoordinateId { get; set; }
    public int CommunicateId { get; set; }
}
