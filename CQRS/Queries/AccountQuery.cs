namespace Sc3S.CQRS.Queries;

public class AccountQuery
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? RoleId { get; set; }
}
