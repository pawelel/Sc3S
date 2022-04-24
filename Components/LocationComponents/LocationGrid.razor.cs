﻿using Sc3S.DTO;

namespace Sc3S.Components.LocationComponents;
public partial class LocationGrid
{
    private List<LocationDto> _locations = new();

    private string _searchString = string.Empty;

    private Func<LocationDto, bool> AreaFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        if (x.Area?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        return false;
    };

    private Func<LocationDto, bool> SpaceFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Area?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        if (x.Space?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        return false;

    };
    private Func<LocationDto, bool> CoordinateFilter => x =>
    {
        return _searchString != null && typeof(LocationDto).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
    };
    
    
}
