using Microsoft.EntityFrameworkCore;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Interfaces;
using ReserveIt.DAL.Context;
using ReserveIt.DAL.Models;

namespace ReserveIt.BLL.Services;

public class BookingService : IBookingService
{
    private readonly ReserveItDbContext _context;

    public BookingService(ReserveItDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Select(b => new BookingDTO(
                b.Id,
                b.RoomId,
                b.Room!.Name,
                b.StartTime,
                b.EndTime,
                b.BookedBy))
            .ToListAsync();
    }

    public async Task<BookingDTO?> GetBookingByIdAsync(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        return booking is null
            ? null
            : new BookingDTO(
                booking.Id,
                booking.RoomId,
                booking.Room?.Name ?? "Unknown",
                booking.StartTime,
                booking.EndTime,
                booking.BookedBy);
    }

    public async Task<(bool IsSuccess, string? ErrorMessage, BookingDTO? Booking)> CreateBookingAsync(CreateBookingDTO BookingDTO)
    {
        if (BookingDTO.EndTime <= BookingDTO.StartTime)
        {
            return (false, "End time must be after start time", null);
        }

        if (BookingDTO.StartTime < DateTime.Now)
        {
            return (false, "Cannot book in the past", null);
        }

        var room = await _context.Rooms.FindAsync(BookingDTO.RoomId);
        if (room is null)
        {
            return (false, "Room not found", null);
        }

        var isRoomAvailable = await IsRoomAvailable(BookingDTO.RoomId, BookingDTO.StartTime, BookingDTO.EndTime);
        if (!isRoomAvailable)
        {
            return (false, "Room is already booked for this time", null);
        }

        var booking = new Booking
        {
            RoomId = BookingDTO.RoomId,
            StartTime = BookingDTO.StartTime,
            EndTime = BookingDTO.EndTime,
            BookedBy = BookingDTO.BookedBy
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return (true, null, new BookingDTO(
            booking.Id,
            booking.RoomId,
            room.Name,
            booking.StartTime,
            booking.EndTime,
            booking.BookedBy));
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking is null)
            return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> IsRoomAvailable(int roomId, DateTime startTime, DateTime endTime)
    {
        return !await _context.Bookings
            .AnyAsync(b => b.RoomId == roomId &&
                           ((b.StartTime <= startTime && b.EndTime > startTime) ||
                           (b.StartTime < endTime && b.EndTime >= endTime) ||
                           (b.StartTime >= startTime && b.EndTime <= endTime)));
    }
}