using Selu383.SP24.Api.Features;

namespace SP24.P02.Web.Features.Users
{
    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string[] Roles { get; set; }
        public string Password { get; set; }
    }
}