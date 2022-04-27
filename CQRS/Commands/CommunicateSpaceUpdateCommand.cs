namespace Sc3S.CQRS.Commands;

public class CommunicateSpaceUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int SpaceId { get; set; }
}