namespace Sc3S.CQRS.Queries;

public class CommunicateAssetQuery : BaseQuery
{
    public int AssetId { get; set; }
    public int CommunicateId { get; set; }
}