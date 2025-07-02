using Microsoft.EntityFrameworkCore;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Exceptions;
using ReserveIt.BLL.Interfaces;
using ReserveIt.DAL.Context;
using ReserveIt.DAL.Models;

namespace ReserveIt.BLL.Services;

public class RoomService : IRoomService
{
    private readonly ReserveItDbContext _context;

    public RoomService(ReserveItDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
    {
        return await _context.Rooms
            .Select(r => new RoomDTO(r.Id, r.Name, r.Capacity))
            .ToListAsync();
    }

    public async Task<RoomDTO> GetRoomByIdAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        
        return room is null
            ? throw new NotFoundException($"Room with ID {id} not found")
            : new RoomDTO(room.Id, room.Name, room.Capacity);
    }

    public async Task<RoomDTO> CreateRoomAsync(CreateRoomDTO roomDto)
    {
        var room = new Room
        {
            Name = roomDto.Name,
            Capacity = roomDto.Capacity
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomDTO(room.Id, room.Name, room.Capacity);
    }
}