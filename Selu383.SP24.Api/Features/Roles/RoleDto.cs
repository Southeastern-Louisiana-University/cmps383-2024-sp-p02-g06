namespace Selu383.SP24.Api.Features;

public class RoleDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
}