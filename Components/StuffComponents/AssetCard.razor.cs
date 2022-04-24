using Microsoft.AspNetCore.Components;

using Sc3S.CQRS.Queries;

namespace Sc3S.Components.StuffComponents;
public partial class AssetCard : ComponentBase
{
    [Parameter] public AssetDisplayQuery AssetModel { get; set; } = new();
}
