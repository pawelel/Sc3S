using AutoMapper;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.CQRS.Commands;
using Sc3S.Services;

namespace Sc3S.Components.LocationComponents;

public partial class PlantForm : ComponentBase
{
    private PlantUpdateCommand _plantUpdate = new();
    private MudForm _form = new();

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    private ILocationService LocationService { get; set; } = default!;

    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Parameter] public int PlantId { get; set; }
    [Inject] public IMapper Mapper { get; set; } = default!;

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    protected override async Task OnInitializedAsync()
    {
        if (PlantId > 0)
        {
            await GetPlant();
        }
    }

    private async Task GetPlant()
    {
        var result = await LocationService.GetPlantById(PlantId);
        if (result.IsSuccess)
        {
            _plantUpdate = Mapper.Map<PlantUpdateCommand>(result.Value);
            return;
        }
        _plantUpdate = new();
    }

    private async Task HandleSave()
    {
        if (PlantId > 0)
        {
            var result = await LocationService.UpdatePlant(_plantUpdate);
            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }
            Snackbar.Add(result.Message, Severity.Error);
        }
        else
        {
            var result = await LocationService.CreatePlant(_plantUpdate);
            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }
            Snackbar.Add(result.Message, Severity.Error);
        }

        await _form.Validate();
        if (_form.IsValid)
            try
            {
                await LocationService.CreatePlant(_plantUpdate);
                Snackbar.Add("Pomyślnie dodano nowy zakład", Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
    }
}