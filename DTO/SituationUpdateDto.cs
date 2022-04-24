namespace Sc3S.DTO;
public class SituationUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<SituationQuestionDto> SituationQuestions { get; set; } = new();
    public virtual List<CategorySituationDto> CategorySituations { get; set; } = new();
    public virtual List<SituationDetailDto> SituationDetails { get; set; } = new();
    public virtual List<SituationParameterDto> SituationParameters { get; set; } = new();
    public virtual List<DeviceSituationDto> DeviceSituations { get; set; } = new();
}
