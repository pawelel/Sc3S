namespace Sc3S.Entities;

public abstract class BaseEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; } 
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}