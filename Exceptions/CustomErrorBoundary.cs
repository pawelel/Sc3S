using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Sc3S.Exceptions;

public class CustomErrorBoundary : ErrorBoundary
{
    [Inject]
    private IWebHostEnvironment Env { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        if (Env.IsDevelopment())
        {
            return base.OnErrorAsync(exception);
        }
        return Task.CompletedTask;
    }
}