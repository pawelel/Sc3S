namespace Sc3S.CQRS.Queries;

public class ModelFlat : BaseQuery
{
    public int ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<ModelParameterQuery> ModelParameters { get; set; } = new();
}