namespace Sc3S.CQRS.Commands;

public class CommunicateCoordinateUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int CoordinateId { get; set; }
}