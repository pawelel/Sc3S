using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using Sc3S.CQRS.Queries;

using System.Security.Claims;

namespace Sc3S.Components.Authentication;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _browserStorage;
    private readonly ILogger<AuthStateProvider> _logger;

    public AuthStateProvider(ProtectedLocalStorage browserStorage, ILogger<AuthStateProvider> logger)
    {
        _browserStorage = browserStorage;
        _logger = logger;
    }

    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userStorageResult = await _browserStorage.GetAsync<UserSession>("UserSession");
            var userSession = userStorageResult.Success ? userStorageResult.Value : null;
            if (userSession == null)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, userSession.UserName)
            };
            foreach (var item in userSession.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims
        , "CustomAuth"));
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get authentication state");
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }
    public async Task UpdateAuthenticationState(UserSession userSession)
    {
        ClaimsPrincipal claimsPrincipal;
        if (userSession != null)
        {
            await _browserStorage.SetAsync("UserSession", userSession);
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, userSession.UserName)
            };
            foreach (var item in userSession.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
        else
        {
            await _browserStorage.DeleteAsync("UserSession");
            claimsPrincipal = _anonymous;
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task Logout()
    {
        await UpdateAuthenticationState(null!);
    }
}
