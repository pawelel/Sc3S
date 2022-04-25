﻿using AutoMapper;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Infrastructure;

using MudBlazor;

using Sc3S.CQRS.Commands;
using Sc3S.Services;
using Sc3S.Validators;

namespace Sc3S.Components.StuffComponents;
public partial class DeviceForm : ComponentBase
{

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [Inject]
    private IStuffService StuffService { get; set; } = default!;
    [Inject] IMapper Mapper { get; set; } = default!;

    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Parameter] public int DeviceId { get; set; }
    private DeviceUpdateCommand _device = new();

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    async Task GetDevice(int deviceId)
    {
        var result = await StuffService.GetDeviceById(deviceId);
        if (result.Success)
        {
            _device = Mapper.Map<DeviceUpdateCommand>(result.Data);
        }
        else
        {
            Snackbar.Add(result.Message, MudBlazor.Severity.Error);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (DeviceId > 0)
        {
            await GetDevice(DeviceId);
        }
    }
    private async Task HandleSave()
    {


        if (DeviceId > 0)
        {
            var result = await StuffService.UpdateDevice(_device);
            if (result.Success)
            {
                Snackbar.Add(result.Message, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }
            Snackbar.Add(result.Message, Severity.Error);
        }
        else
        {
          var result =  await StuffService.CreateDevice(_device);
            if (result.Success)
            {
                Snackbar.Add(result.Message, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }
            Snackbar.Add(result.Message, Severity.Error);
        }
        

    }
}
