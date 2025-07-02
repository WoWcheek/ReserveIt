namespace ReserveIt.DAL.Models;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string BookedBy { get; set; } = string.Empty;

    public Room Room { get; set; } = null!;
}