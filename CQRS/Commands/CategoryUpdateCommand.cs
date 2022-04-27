namespace Sc3S.CQRS.Commands;

public class CategoryUpdateCommand : BaseCommand
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}