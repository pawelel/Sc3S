using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.CQRS.Queries;
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
    private IEnumerable<DeviceQuery> _devices = new List<DeviceQuery>();
    protected override async Task OnInitializedAsync()
    {
        await GetDevices();
    }
    private string _search = string.Empty;
    private async Task GetDevices()
    {
        var result = await StuffService.GetDevices();
        if (result.Success)
        {
            _devices = result.Data!;
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
            _devices = new List<DeviceQuery>();
        }
    }

    private bool Search(DeviceQuery device)
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
        
        var dialog = DialogService.Show<DeviceForm>("Edycja sprzętu", parameters);
        var result = await dialog.Result;
        if (result.Cancelled == false)
        {
            await GetDevices();
        }
    }
    private async Task MarkDelete(int id)
    {
      var result =  await StuffService.MarkDeleteDevice(id);
        Snackbar.Add(result.Message, Severity.Warning);
        await GetDevices();
    }

    private async Task Delete(int id)
    {
       var result = await StuffService.DeleteDevice(id);
        Snackbar.Add(result.Message, Severity.Warning);
        await GetDevices();
    }
}
