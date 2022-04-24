using Sc3S.CQRS.Queries;

namespace Sc3S.Components.LocationComponents;

public partial class LocationSearch
{
    private readonly IEnumerable<LocationQuery> _locations = new List<LocationQuery>() {
       new LocationQuery() {
           Area = "Area 1",
           Space =  "Space 1",
           Coordinate = "Location 1"
       }
   };
    string _searchString = string.Empty;
    private Func<LocationQuery, bool> LocationFilter => x =>
    {
        return _searchString != null && typeof(LocationQuery).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
    };
}


