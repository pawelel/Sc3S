using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.CQRS.Queries;
using Sc3S.Services;

using static MudBlazor.CategoryTypes;

namespace Sc3S.Components.StuffComponents;

public partial class DeviceGrid : Microsoft.AspNetCore.Components.ComponentBase
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IStuffService StuffService { get; set; } = default!;
    public string UserName = "";
    [Inject] private IDialogService DialogService { get; set; } = default!;
    private IEnumerable<DeviceQuery> _devices = new List<DeviceQuery>();

    protected override async Task OnInitializedAsync()
    {
        await GetDevices();
    }

    private string _search = string.Empty;

    private async Task GetDevices()
    {
        var result = await StuffService.GetDevices();
        if (result.IsSuccess)
        {
            _devices = result.Value!;
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
            _devices = new List<DeviceQuery>();
        }
    }

    private Func<DeviceQuery, bool> QuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_search))
        {
            return true;
        }
        return x.Name.ToLower().Contains(_search.ToLower());
    };

    private async Task Edit(int id)
    {
        var parameters = new DialogParameters
        {
            { "DeviceId", id },
            {"UserName", UserName }
        };

        var dialog = DialogService.Show<DeviceForm>("Edycja sprzętu", parameters);
        var result = await dialog.Result;
        if (result.Cancelled == false)
        {
            await GetDevices();
        }
    }

    private async Task MarkDelete(string userName, int id)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            var result = await StuffService.MarkDeleteDevice(userName, id);
            Snackbar.Add(result.Message, Severity.Warning);
            await GetDevices();
        }
    }

    private async Task Delete(int id)
    {
        var result = await StuffService.DeleteDevice(id);
        Snackbar.Add(result.Message, Severity.Warning);
        await GetDevices();
    }
}