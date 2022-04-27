using Microsoft.AspNetCore.Identity;

namespace Sc3S.Entities;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();
    public virtual ICollection<IdentityUserRole<string>> UserRoles { get; } = new List<IdentityUserRole<string>>();

}
