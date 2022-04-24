using Sc3S.Enumerations;
namespace Sc3S.CQRS.Commands;
public class CommunicateUpdateCommand
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int CommunicateId { get; set; }
}
