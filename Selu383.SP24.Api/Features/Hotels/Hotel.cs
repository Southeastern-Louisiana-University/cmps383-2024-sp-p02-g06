using System.ComponentModel.DataAnnotations;

namespace Selu383.SP24.Api.Features.Hotels;

public class Hotel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; }

    [Required]
    public string Address { get; set; }

    public User? Manager { get; set; }
}
