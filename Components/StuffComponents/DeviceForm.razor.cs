using FluentValidation;

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

    MudForm _form=new();
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [Inject]
    private IStuffService StuffService { get; set; } = default!;
    [Inject] IValidator<DeviceUpdateCommand> Validator { get; set; } = default!;
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await Validator.ValidateAsync(ValidationContext<DeviceUpdateCommand>
            .CreateWithOptions((DeviceUpdateCommand)model, x => x.IncludeProperties(propertyName)));

       if (result.IsValid)
            return Array.Empty<string>();

        return result.Errors.Select(e => e.ErrorMessage);
    };
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Parameter] public int DeviceId { get; set; }
    private DeviceUpdateCommand _device = new();
   
    private void Cancel()
    {
        MudDialog.Cancel();
    }

    
    
    protected override async Task OnInitializedAsync()
    {
        if (DeviceId > 0)
        {
            _device = await StuffService.GetDeviceToUpdateById(DeviceId);
        }
    }
    private async Task HandleSave()
    {
        await _form.Validate();
        if (_form.IsValid) { 

        if (DeviceId > 0)
        {
            await StuffService.UpdateDevice(DeviceId, _device);
        }
        else
        {
            await StuffService.CreateDevice(_device);
        }
        MudDialog.Close(DialogResult.Ok(true));
            }
    }
}
