namespace Sc3S.CQRS.Commands;

public class CommunicateModelUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int ModelId { get; set; }
}