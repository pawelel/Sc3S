namespace Sc3S.CQRS.Queries;

public class AreaFlat : BaseQuery
{
    public int AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}