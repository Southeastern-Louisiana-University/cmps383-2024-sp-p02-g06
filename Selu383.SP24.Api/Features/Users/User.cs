using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace Selu383.SP24.Api.Features;

public class User : IdentityUser<int>
{ 
    public int Id { get; set; }

    public virtual ICollection<UserRole> Roles { get; } = new List<UserRole>();
}
