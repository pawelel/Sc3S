namespace Sc3S.Entities;

public class Account
{
    public string UserId {get; set;} = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? RoleId { get; set; }
    public Role Role { get; set; } = new();
}
