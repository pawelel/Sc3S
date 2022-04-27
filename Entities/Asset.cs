﻿using Sc3S.Enumerations;

namespace Sc3S.Entities;

public class Asset : BaseEntity
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int ModelId { get; set; }
    public virtual Model Model { get; set; } = new();
    public virtual Coordinate Coordinate { get; set; } = new();
    public int CoordinateId { get; set; }
    public virtual List<AssetCategory> AssetCategories { get; set; } = new();
    public virtual List<AssetDetail> AssetDetails { get; set; } = new();
    public virtual List<CommunicateAsset> CommunicateAssets { get; set; } = new();
    public virtual List<AssetSituation> AssetSituations { get; set; } = new();
}