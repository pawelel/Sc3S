namespace Sc3S.CQRS.Queries;

public class BaseQuery
{
    public string UpdatedBy { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}