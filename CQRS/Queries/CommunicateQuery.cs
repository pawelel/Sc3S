using Sc3S.Enumerations;

namespace Sc3S.CQRS.Queries;

public class CommunicateQuery : BaseQuery
{
    public int CommunicateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public virtual List<CommunicateAreaQuery> CommunicateAreas { get; set; } = new();
    public virtual List<CommunicateAssetQuery> CommunicateAssets { get; set; } = new();
    public virtual List<CommunicateCoordinateQuery> CommunicateCoordinates { get; set; } = new();
    public virtual List<CommunicateDeviceQuery> CommunicateDevices { get; set; } = new();
    public virtual List<CommunicateModelQuery> CommunicateModels { get; set; } = new();
    public virtual List<CommunicateSpaceQuery> CommunicateSpaces { get; set; } = new();
    public virtual List<CommunicateCategoryQuery> CommunicateCategories { get; set; } = new();
}