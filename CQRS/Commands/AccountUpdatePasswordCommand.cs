namespace Sc3S.CQRS.Commands;

public class AccountUpdatePasswordCommand : BaseCommand
{
    public string UserId { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;
}