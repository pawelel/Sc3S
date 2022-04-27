using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.CQRS.Queries;
using Sc3S.Services;

namespace Sc3S.Components.AdminComponents;

public partial class AccountDisplayGrid : ComponentBase
{
    private IEnumerable<AccountDisplayQuery> _accounts = new List<AccountDisplayQuery>();
    [Inject] private IAccountService AccountService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var result = await AccountService.GetAccounts();
        if (result.IsSuccess)
        {
            _accounts = result.Value!;
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Warning);
            _accounts = new List<AccountDisplayQuery>();
        }
    }
}