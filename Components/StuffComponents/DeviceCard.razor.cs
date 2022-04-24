using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.Entities;
using Sc3S.Services;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc3S.Components.StuffComponents;
public partial class DeviceCard : ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] IStuffService StuffService { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;
    private IEnumerable<Device> _devices = new List<Device>();
    protected override async Task OnInitializedAsync()
    {
        await GetDevices();
    }
    private string _search = string.Empty;
    private async Task GetDevices()
    {
        _devices = await StuffService.GetDevices();
    }

    private bool Search(Device device)
    {
        if (string.IsNullOrWhiteSpace(_search))
        {
            return true;
        }
        return device.Name.ToLower().Contains(_search.ToLower());
    }
    
    private async Task Edit(int id)
    {
        var parameters = new DialogParameters
        {
            { "DeviceId", id }
        };
        //_device = _devices.FirstOrDefault(d => d.DeviceId==id)??new();
        var dialog = DialogService.Show<DeviceForm>("Edycja sprzętu", parameters);
        var result = await dialog.Result;
        if (result.Cancelled == false)
        {
            await GetDevices();
            Snackbar.Add("Zapisano zmiany");
        }
    }
    private async Task MarkDelete(int id)
    {
        await StuffService.MarkDeleteDevice(id);
        Snackbar.Add("Sprzęt oznaczony do usunięcia", Severity.Warning);
    }

    private async Task Delete(int id)
    {
        await StuffService.DeleteDevice(id);
        await GetDevices();
    }

    //private async Task Save()
    //{
    //    await StuffService.SaveDevice(_device);
    //    _device = new();
    //    Snackbar.Add("Sprzęt zapisany", Severity.Success);
    //    await GetDevices();
    //}
}
