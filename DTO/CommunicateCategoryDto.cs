using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CommunicateCategoryDto : BaseDto
{
    public int CommunicateId { get; set; }
    public int CategoryId { get; set; }
}
