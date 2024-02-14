using Selu383.SP24.Api.Features;

namespace Selu383.SP24.Api.Features;

public class UserCreateDto
{
    public string? UserName { get; set; }
    public string[]? Roles { get; set; }
    public string? Password { get; set; }
}