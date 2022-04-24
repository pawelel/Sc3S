﻿using Sc3S.DTO;

namespace Sc3S.DTO;
public class PlantDto : BaseDto
{
    public int PlantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AreaDto> Areas { get; set; } = new();
}
