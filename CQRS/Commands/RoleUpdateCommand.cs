namespace Sc3S.CQRS.Commands
{
    public class RoleUpdateCommand : BaseCommand
    {
        public string Name { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}