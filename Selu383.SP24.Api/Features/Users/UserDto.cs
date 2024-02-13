using Selu383.SP24.Api.Features;

namespace Selu383.SP24.Api.Features;

public class UserDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public ICollection<string> Roles { get; set; }
}