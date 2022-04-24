namespace Sc3S.DTO;
public class QuestionWithSituationsDto
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationDto> Situations { get; set; } = new();
}
