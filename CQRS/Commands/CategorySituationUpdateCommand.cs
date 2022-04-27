namespace Sc3S.CQRS.Commands;

public class CategorySituationUpdateCommand : BaseCommand
{
    public int CategoryId { get; set; }
    public int SituationId { get; set; }
}
