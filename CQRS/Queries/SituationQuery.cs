namespace Sc3S.CQRS.Queries;

public class SituationQuery : BaseQuery
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<SituationQuestionQuery> SituationQuestions { get; set; } = new();
    public virtual List<SituationDetailQuery> SituationDetails { get; set; } = new();
    public virtual List<SituationParameterQuery> SituationParameters { get; set; } = new();
    public virtual List<CategorySituationQuery> CategorySituations { get; set; } = new();
    public virtual List<DeviceSituationQuery> DeviceSituations { get; set; } = new();
    public List<AssetSituationQuery> AssetSituations { get; set; } = new();
    public virtual List<CommunicateCategoryQuery> CommunicateCategories { get; set; } = new();
}