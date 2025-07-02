using System.ComponentModel.DataAnnotations;

namespace ReserveIt.BLL.DTOs;

public record CreateBookingDTO
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string BookedBy { get; set; } = string.Empty;
}