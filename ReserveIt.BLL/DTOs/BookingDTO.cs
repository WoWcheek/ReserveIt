namespace ReserveIt.BLL.DTOs;

public record BookingDTO(int Id, int RoomId, string RoomName, DateTime StartTime, DateTime EndTime, string BookedBy);