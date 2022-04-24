namespace Sc3S.CQRS.Queries;
public class BaseDto
{
    public string UserId { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}
