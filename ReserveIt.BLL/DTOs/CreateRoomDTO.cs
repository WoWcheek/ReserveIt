using System.ComponentModel.DataAnnotations;

namespace ReserveIt.BLL.DTOs;

public record CreateRoomDTO
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public int Capacity { get; set; }
}
