using Microsoft.EntityFrameworkCore;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Exceptions;
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

    public async Task<BookingDTO> GetBookingByIdAsync(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);

        return booking is null
            ? throw new NotFoundException($"Booking with ID {id} not found")
            : new BookingDTO(
                booking.Id,
                booking.RoomId,
                booking.Room?.Name ?? string.Empty,
                booking.StartTime,
                booking.EndTime,
                booking.BookedBy);
    }

    public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDto)
    {
        if (bookingDto.EndTime <= bookingDto.StartTime)
        {
            throw new BadRequestException("End time must be after start time");
        }

        if (bookingDto.StartTime < DateTime.Now)
        {
            throw new BadRequestException("Cannot book in the past");
        }

        var room = await _context.Rooms.FindAsync(bookingDto.RoomId);
        if (room is null)
        {
            throw new NotFoundException($"Room with ID {bookingDto.RoomId} not found");
        }

        var isRoomAvailable = await IsRoomAvailable(bookingDto.RoomId, bookingDto.StartTime, bookingDto.EndTime);
        if (!isRoomAvailable)
        {
            throw new ConflictException("Room is already booked for this time");
        }

        var booking = new Booking
        {
            RoomId = bookingDto.RoomId,
            StartTime = bookingDto.StartTime,
            EndTime = bookingDto.EndTime,
            BookedBy = bookingDto.BookedBy
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return new BookingDTO(
            booking.Id,
            booking.RoomId,
            room.Name,
            booking.StartTime,
            booking.EndTime,
            booking.BookedBy);
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking is null)
            throw new NotFoundException($"Booking with ID {id} not found");

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