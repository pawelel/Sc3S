using MediatR;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using Sc3S.Components.Authentication;
using Sc3S.Data;
using Sc3S.Entities;
using System.Reflection;

namespace Sc3S.Extensions;

public static class ServiceExtensions
{

    public static void ServiceWrapper(this IServiceCollection services)
    {
        services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<CustomAuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<CustomAuthenticationStateProvider>());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
        services.AddMudServices();
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}
