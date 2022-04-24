namespace Sc3S.CQRS.Commands;
public class ParameterUpdateCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ParameterId { get; set; }
}
