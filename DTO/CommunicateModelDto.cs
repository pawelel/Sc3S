using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CommunicateModelDto : BaseDto
{
    public int ModelId { get; set; }
    public int CommunicateId { get; set; }
}
