using System.ComponentModel.DataAnnotations;

namespace ReserveIt.BLL.DTOs;

public record LoginDTO
{
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Password { get; set; } = string.Empty;
}