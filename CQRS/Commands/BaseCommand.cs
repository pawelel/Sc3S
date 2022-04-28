namespace Sc3S.CQRS.Commands;

public class BaseCommand
{
    public string UpdatedBy { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
}