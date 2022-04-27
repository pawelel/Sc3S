namespace Sc3S.CQRS.Queries;

public class CoordinateFlat : BaseQuery
{
    public int CoordinateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}