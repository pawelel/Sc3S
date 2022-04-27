namespace Sc3S.CQRS.Commands;

public class CommunicateAreaUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int AreaId { get; set; }
}