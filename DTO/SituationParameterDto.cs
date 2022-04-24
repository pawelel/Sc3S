using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class SituationParameterDto : BaseDto
{
    public int SituationId { get; set; }
    public int ParameterId { get; set; }
}
