﻿using AutoMapper;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Sc3S.CQRS.Commands;
using Sc3S.Entities;
using Sc3S.Services;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sc3S.Components.AdminComponents;
public partial class AccountForm : ComponentBase
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    private IAccountService AccountService { get; set; } = default!;

    [Inject] private IMapper Mapper { get; set; } = default!;
    [Parameter] public string UserName { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Parameter] public string UserId { get; set; } = string.Empty;
    private AccountUpdateCommand _account = new();
    private List<Role> _roles = new();
    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task GetAccount(string userId)
    {
        var result = await AccountService.GetAccountById(userId);
        if (result.IsSuccess)
        {
            _account = Mapper.Map<AccountUpdateCommand>(result.Value);
        }
        else
        {
            Snackbar.Add(result.Message, MudBlazor.Severity.Error);
        }
      
    }

    private async Task GetRoles()
    {
        var result = await AccountService.GetRoles();
        if (result.IsSuccess)
        {
            _roles = result.Value!;
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(_account.UserId))
        {
            await AccountService.GetAccountById(_account.UserId);
        }
        await GetRoles();
    }

    private async Task HandleSave()
    {
        _account.UpdatedBy = UserName;

        if (!string.IsNullOrEmpty(_account.UserId))
        {
            var result = await AccountService.UpdateAccount(_account);
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
            var result = await AccountService.CreateAccount(_account);
            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }
            Snackbar.Add(result.Message, Severity.Error);
        }
    }
}
