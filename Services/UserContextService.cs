
using Microsoft.AspNetCore.Http;

using System.Security.Claims;
namespace Sc3S.Services;
public interface IUserContextService
{
    ClaimsPrincipal User { get; }
    string UserId { get; }
    
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);


}
