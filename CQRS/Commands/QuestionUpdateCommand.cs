namespace Sc3S.CQRS.Commands;

public class QuestionUpdateCommand : BaseCommand
{
    public string Name { get; set; } = string.Empty;
    public int QuestionId { get; set; }
    public string UserName { get; set; } = string.Empty;
}