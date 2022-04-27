namespace Sc3S.CQRS.Queries;

public class DetailQuery : BaseQuery
{
    public int DetailId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}