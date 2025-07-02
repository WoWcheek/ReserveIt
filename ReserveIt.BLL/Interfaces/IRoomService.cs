using ReserveIt.BLL.DTOs;

namespace ReserveIt.BLL.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDTO>> GetAllRoomsAsync();
    Task<RoomDTO> GetRoomByIdAsync(int id);
    Task<RoomDTO> CreateRoomAsync(CreateRoomDTO roomDto);
}