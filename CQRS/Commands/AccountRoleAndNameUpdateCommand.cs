namespace Sc3S.CQRS.Commands;

public class AccountRoleAndNameUpdateCommand : BaseCommand
{
    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}
