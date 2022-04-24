namespace Sc3S.CQRS.Commands;
public class QuestionUpdateCommand
{
    public string Name { get; set; } = string.Empty;
    public int QuestionId { get; set; }
}
