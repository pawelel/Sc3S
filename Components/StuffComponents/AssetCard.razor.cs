using Microsoft.AspNetCore.Components;

using Sc3S.DTO;

namespace Sc3S.Components.StuffComponents;
public partial class AssetCard : ComponentBase
{
    [Parameter] public AssetDisplayDto AssetModel { get; set; } = new();
}
