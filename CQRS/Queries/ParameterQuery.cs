namespace Sc3S.CQRS.Queries;
public class ParameterQuery : BaseDto
{
    public int ParameterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
