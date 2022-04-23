using MediatR;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using Sc3S.Components.Authentication;
using Sc3S.Data;
using Sc3S.Entities;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(c => c.DetailedErrors = true);
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<CustomAuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContextFactory<Sc3SContext>(options => 
{
    options.UseSqlServer(cs);
    options.EnableSensitiveDataLogging();
});
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddMudServices();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();