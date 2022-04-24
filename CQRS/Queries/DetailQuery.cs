namespace Sc3S.CQRS.Queries;
public class DetailQuery : BaseDto
{
    public int DetailId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
