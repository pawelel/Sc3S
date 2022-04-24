﻿using Sc3S.Enumerations;
namespace Sc3S.DTO;
public class AssetWithDetailsDisplayDto : BaseDto
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int ModelId { get; set; }
    public int CoordinateId { get; set; }
    public virtual List<AssetDetailDisplayDto> Details { get; set; } = new();
}
