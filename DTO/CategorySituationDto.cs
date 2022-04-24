using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class CategorySituationDto : BaseDto
{
    public int CategoryId { get; set; }
    public int SituationId { get; set; }
}
