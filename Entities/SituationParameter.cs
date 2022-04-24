namespace Sc3S.Entities;
public class SituationParameter : BaseEntity
{
    public int SituationId { get; set; }
    public int ParameterId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public virtual Parameter Parameter { get; set; } = new();
}
