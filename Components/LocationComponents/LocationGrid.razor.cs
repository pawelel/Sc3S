using Sc3S.CQRS.Queries;

namespace Sc3S.Components.LocationComponents;

public partial class LocationGrid
{
    private List<LocationQuery> _locations = new();

    private string _searchString = string.Empty;

    private Func<LocationQuery, bool> AreaFilter => x =>
     {
         if (string.IsNullOrWhiteSpace(_searchString))
             return true;
         if (x.Area?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
             return true;
         return false;
     };

    private Func<LocationQuery, bool> SpaceFilter => x =>
     {
         if (string.IsNullOrWhiteSpace(_searchString))
             return true;

         if (x.Area?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
             return true;
         if (x.Space?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
             return true;
         return false;
     };

    private Func<LocationQuery, bool> CoordinateFilter => x =>
     {
         return _searchString != null && typeof(LocationQuery).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
     };
}