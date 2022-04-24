﻿using Sc3S.Enumerations;

namespace Sc3S.CQRS.Queries;
public class AssetDisplayQuery : BaseDto
{

    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public string CoordinateName { get; set; } = string.Empty;
    public int CoordinateId { get; set; }
    public string SpaceName { get; set; } = string.Empty;
    public SpaceType SpaceType { get; set; }
    public int SpaceId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public int AreaId { get; set; }
    public string PlantName { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public virtual List<AssetDetailDisplayQuery> Details { get; init; } = new();
    public int DeviceId { get; set; }
    public virtual List<AssetCategoryDisplayQuery> Categories { get; init; } = new();
    public bool ShowDetails { get; set; }
    public virtual List<ModelParameterDisplayQuery> Parameters { get; init; } = new();
}
