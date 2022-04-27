namespace Sc3S.CQRS.Queries;

public class ModelParameterQuery : BaseQuery
{
    public string Value { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public int ParameterId { get; set; }
}