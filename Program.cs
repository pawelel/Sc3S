
using MediatR;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MudBlazor;
using MudBlazor.Services;

using Sc3S.Components.Authentication;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Middleware;
using Sc3S.Services;

using Serilog;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<Sc3SContext>(options =>
{
    options.UseSqlServer(cs);
});
// Add builder.Services to the container.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


builder.Services.AddRazorPages();
//builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ILocationService, LocationService>();
builder.Services.AddTransient<ICommunicateService, CommunicateService>();
builder.Services.AddTransient<ISituationService, SituationService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthStateProvider, AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<AuthStateProvider>());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = false;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
});
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sc3S API V1");
//});
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();