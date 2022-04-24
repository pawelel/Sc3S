namespace Sc3S.CQRS.Commands;
public class SituationUpdateCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SituationId { get; set; }
}
