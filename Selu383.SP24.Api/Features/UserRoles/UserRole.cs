using Microsoft.AspNetCore.Identity;

namespace Selu383.SP24.Api.Features
{
    public class UserRole : IdentityUserRole<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
