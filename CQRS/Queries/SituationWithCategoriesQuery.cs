namespace Sc3S.CQRS.Queries;
public class SituationWithCategoriesQuery : BaseDto
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<CategoryQuery> Categories { get; set; } = new();
}
