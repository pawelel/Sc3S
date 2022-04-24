using Sc3S.DTO;

namespace Sc3S.CQRS.Queries;
/// <summary>
///     Trouble shooting question not from DB
/// </summary>
public class QuestionQuery : BaseDto
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationQuestionDto> SituationQuestions { get; set; } = new();
}
