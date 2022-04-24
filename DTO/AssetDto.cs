﻿using Sc3S.Enumerations;
namespace Sc3S.DTO;
public class AssetDto : BaseDto
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int ModelId { get; set; }
    public int CoordinateId { get; set; }
    public virtual List<AssetDetailDto> AssetDetails { get; set; } = new();
    public virtual List<AssetCategoryDto> AssetCategories { get; set; } = new();
    public virtual List<CommunicateAssetDto> CommunicateAssets { get; set; } = new();
    public List<AssetSituationDto> AssetSituations { get; set; } = new();
}
