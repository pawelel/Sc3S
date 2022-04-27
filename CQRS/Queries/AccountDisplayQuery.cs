namespace Sc3S.CQRS.Queries;

public class AccountDisplayQuery : BaseQuery
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}