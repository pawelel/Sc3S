namespace Sc3S.CQRS.Commands;
public class DetailUpdateCommand
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public int DetailId { get; set; }
}
