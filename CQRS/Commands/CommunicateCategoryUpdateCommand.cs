namespace Sc3S.CQRS.Commands;

public class CommunicateCategoryUpdateCommand : BaseCommand
{
    public int CommunicateId { get; set; }
    public int CategoryId { get; set; }
}