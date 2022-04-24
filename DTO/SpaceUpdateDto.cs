﻿using Sc3S.Enumerations;
namespace Sc3S.DTO;
public class SpaceUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual List<CoordinateDto> Coordinates { get; set; } = new();
    public SpaceType SpaceType { get; set; }

    public virtual List<CommunicateSpaceDto> CommunicateSpaces { get; set; } = new();
}
