namespace Sc3S.Entities;

public class Role : BaseEntity
{
    public string RoleId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}