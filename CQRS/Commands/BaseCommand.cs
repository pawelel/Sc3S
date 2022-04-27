namespace Sc3S.CQRS.Commands;

public class BaseCommand
{
    public string UpdatedBy { get; set; } = string.Empty;
}