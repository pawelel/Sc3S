using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.CQRS.Queries;
using Sc3S.Enumerations;
using Sc3S.Services;

namespace Sc3S.Components.StuffComponents;

public partial class AssetSearch : ComponentBase
{
    private IEnumerable<AssetDisplayQuery> _assets = new List<AssetDisplayQuery>();
    private IEnumerable<AssetDisplayQuery> _filteredAssets = new List<AssetDisplayQuery>();
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    private string _searchString = string.Empty;
    private string _selectedFilters = string.Empty;
    [Inject] private IStuffService StuffService { get; set; } = default!;

    private Func<AssetDisplayQuery, bool> AssetFilter => x =>
     {
         return _searchString != null && typeof(AssetDisplayQuery).GetProperties().Any(p => p.GetValue(x)?.ToString()?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
     };

    protected override async Task OnInitializedAsync()
    {
        await GetAssets();
        _filteredAssets = _assets;
    }

    private async Task GetAssets()
    {
        var result = await StuffService.GetAssetDisplays();
        if (result.IsSuccess)
        {
            _assets = result.Value!;
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
            _assets = new List<AssetDisplayQuery>();
        }
    }

    private static void ShowBtnPress(AssetDisplayQuery aDisplayDto)
    {
        aDisplayDto.ShowDetails = !aDisplayDto.ShowDetails;
    }

    private void ResetAssets()
    {
        _filteredAssets = _assets;
        _selectedFilters = string.Empty;
    }

    private void FilterByCategory(string category)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.Categories!.Any(c => c.Name == category));
        if (!_selectedFilters.Contains(category))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {category} ({count})";
        }
    }

    private void FilterByStatus(Status status)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.Status == status);
        if (!_selectedFilters.Contains(status.ToString()))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {status} ({count})";
        }
    }

    private void FilterByModel(string model)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a =>
        {
            return a.ModelName == model;
        });
        if (!_selectedFilters.Contains(model))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {model} ({count})";
        }
    }

    private void FilterByArea(string area)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.AreaName == area);
        if (!_selectedFilters.Contains(area))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {area} ({count})";
        }
    }

    private void FilterBySpace(string space)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.SpaceName == space);
        if (!_selectedFilters.Contains(space))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {space} ({count})";
        }
    }

    private void FilterByCoordinate(string coordinate)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.CoordinateName == coordinate);
        if (!_selectedFilters.Contains(coordinate))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {coordinate} ({count})";
        }
    }

    private void FilterByDevice(string device)
    {
        var count = 0;
        _filteredAssets = _filteredAssets.Where(a => a.DeviceName == device);
        if (!_selectedFilters.Contains(device))
        {
            count = _filteredAssets.Count();
            _selectedFilters += $" {device} ({count})";
        }
    }
}