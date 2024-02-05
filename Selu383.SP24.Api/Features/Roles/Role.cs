using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace Selu383.SP24.Api.Features
{
    public class Role : IdentityRole<int>
    {
        public int Id { get; set; }

        public virtual ICollection<UserRole> Users { get; } = new List<UserRole>();
    }
}
