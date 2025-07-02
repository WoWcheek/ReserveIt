using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Interfaces;

namespace ReserveIt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDTO>>> GetRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDTO>> GetRoom(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        return Ok(room);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDTO>> CreateRoom(CreateRoomDTO roomDto)
    {
        var room = await _roomService.CreateRoomAsync(roomDto);
        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }
}