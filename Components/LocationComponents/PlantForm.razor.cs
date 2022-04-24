using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.DTO;
using Sc3S.Services;

namespace Sc3S.Components.LocationComponents;
public partial class PlantForm : ComponentBase
{
    private readonly PlantCreateDto _plantCreateDto = new();
    private MudForm _form = new();
    private bool _isOpen;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [Inject]
    private ILocationService LocationService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Parameter] public int PlantId { get; set; }
    private string _name = string.Empty;
    private string _description = string.Empty;

    private void Submit()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }
    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private static IEnumerable<string> MaxNameCharacters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 60 < ch?.Length)
            yield return "Max 59 znaków";
    }
    private static IEnumerable<string> MaxDescriptionCharacters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 250 < ch?.Length)
            yield return "Max 249 znaków";
    }
    public void ToggleOpen()
    {
        _isOpen = !_isOpen;
    }

    protected override async Task OnInitializedAsync()
    {
        if (PlantId != 0)
        {
            var plant = await LocationService.GetPlantById(PlantId);
            _name = plant.Name;
            _description = plant.Description;
        }
    }
    private async Task HandleSave()
    {
        if (PlantId > 0)
        {
            var plantUpdate = new PlantUpdateDto
            {
                Name = _name,
                Description = _description
            };
            await LocationService.UpdatePlant(PlantId, plantUpdate);
        }
        else
        {
            await LocationService.CreatePlant(_plantCreateDto);
        }


        await _form.Validate();
        if (_form.IsValid)
            try
            {
                await LocationService.CreatePlant(_plantCreateDto);
                Snackbar.Add("Pomyślnie dodano nowy zakład", Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }

    }
}
