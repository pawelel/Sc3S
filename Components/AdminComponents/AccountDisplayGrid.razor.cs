using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.Components.StuffComponents;
using Sc3S.CQRS.Queries;
using Sc3S.Services;

namespace Sc3S.Components.AdminComponents;

public partial class AccountDisplayGrid : ComponentBase
{
    private IEnumerable<AccountDisplayQuery> _accounts = new List<AccountDisplayQuery>();
    [Inject] private IAccountService AccountService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;    
    private string _search = string.Empty;
    protected override async Task OnInitializedAsync()
    {
        await GetAccounts();
    }

    private async Task GetAccounts()
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

    async Task MarkDelete(string updatedBy, string userId)
    {
        var result = await AccountService.MarkDeleteAccount(updatedBy, userId);
        if (result.IsSuccess)
        {
            Snackbar.Add(result.Message, Severity.Success);
            await GetAccounts();
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Warning);
        }
    }

    async Task Delete(string userId)
    {
        var result = await AccountService.DeleteAccount(userId);
        if (result.IsSuccess)
        {
            Snackbar.Add(result.Message, Severity.Success);
            await GetAccounts();
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Warning);
        }
    }
    private async Task Edit(string userName, string userId)
    {
        var parameters = new DialogParameters
        {
            { "UserId", userId },
            {"UserName", userName }
        };

        var dialog = DialogService.Show<AccountForm>("Edycja użytkownika", parameters);
        var result = await dialog.Result;
        if (result.Cancelled == false)
        {
            await GetAccounts();
        }
    }
    private Func<AccountDisplayQuery, bool> QuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_search))
        {
            return true;
        }
        return x.UserName.ToLower().Contains(_search.ToLower());
    };
}