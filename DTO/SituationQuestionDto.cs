using Sc3S.CQRS.Queries;

namespace Sc3S.DTO;
public class SituationQuestionDto : BaseDto
{
    public int SituationId { get; set; }
    public int QuestionId { get; set; }
}
