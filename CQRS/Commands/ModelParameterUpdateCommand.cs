namespace Sc3S.CQRS.Commands;

public class ModelParameterUpdateCommand : BaseCommand
{
    public int ModelId { get; set; }
    public int ParameterId { get; set; }
    public string Value { get; set; } = string.Empty;
}