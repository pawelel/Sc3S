namespace Sc3S.CQRS.Queries;

public class CommunicateCategoryQuery : BaseQuery
{
    public int CommunicateId { get; set; }
    public int CategoryId { get; set; }
}