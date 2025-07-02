using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using ReserveIt.DAL.Models;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Exceptions;
using ReserveIt.BLL.Services;
using ReserveIt.DAL.Context;

namespace ReserveIt.BLL.UnitTests;

public class BookingServiceTests
{
    private readonly DbContextOptions<ReserveItDbContext> _options;
    private readonly ReserveItDbContext _context;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _options = new DbContextOptionsBuilder<ReserveItDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ReserveItDbContext(_options);
        _bookingService = new BookingService(_context);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidData_ReturnsBookingDto()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            BookedBy = "Test User"
        };

        // Act
        var result = await _bookingService.CreateBookingAsync(bookingDto);

        // Assert
        result.Should().NotBeNull();
        result.RoomId.Should().Be(room.Id);
        result.RoomName.Should().Be(room.Name);
        result.StartTime.Should().Be(bookingDto.StartTime);
        result.EndTime.Should().Be(bookingDto.EndTime);
        result.BookedBy.Should().Be(bookingDto.BookedBy);

        var bookingInDb = await _context.Bookings.FirstOrDefaultAsync(b => b.RoomId == room.Id);
        bookingInDb.Should().NotBeNull();
        bookingInDb!.StartTime.Should().Be(bookingDto.StartTime);
        bookingInDb.EndTime.Should().Be(bookingDto.EndTime);
        bookingInDb.BookedBy.Should().Be(bookingDto.BookedBy);
    }

    [Fact]
    public async Task CreateBookingAsync_EndTimeBeforeStartTime_ThrowsBadRequestException()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(1), // End time before start time
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_StartTimeInPast_ThrowsBadRequestException()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(-1), // Start time in the past
            EndTime = DateTime.Now.AddHours(1),
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_RoomNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentRoomId = 999;
        var bookingDto = new CreateBookingDTO
        {
            RoomId = nonExistentRoomId,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_RoomAlreadyBooked_ThrowsConflictException()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);

        var existingBooking = new Booking
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(3),
            BookedBy = "Existing User"
        };
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2), // Overlaps with existing booking
            EndTime = DateTime.Now.AddHours(4),
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_OverlappingStartTime_ThrowsConflictException()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);

        var existingBooking = new Booking
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(4),
            BookedBy = "Existing User"
        };
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(3), // Overlaps with existing booking
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_ContainedWithinExistingBooking_ThrowsConflictException()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);

        var existingBooking = new Booking
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(5),
            BookedBy = "Existing User"
        };
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(4),
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_ExistingBookingWithinNewBooking_ThrowsConflictException()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);

        var existingBooking = new Booking
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(3),
            BookedBy = "Existing User"
        };
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(4),
            BookedBy = "Test User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _bookingService.CreateBookingAsync(bookingDto));
    }

    [Fact]
    public async Task CreateBookingAsync_NonConflictingBookings_ReturnsBookingDto()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);

        var existingBooking = new Booking
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            BookedBy = "Existing User"
        };
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2),
            EndTime = DateTime.Now.AddHours(3),
            BookedBy = "Test User"
        };

        // Act
        var result = await _bookingService.CreateBookingAsync(bookingDto);

        // Assert
        result.Should().NotBeNull();
        result.RoomId.Should().Be(room.Id);

        var bookingsInDb = await _context.Bookings.Where(b => b.RoomId == room.Id).ToListAsync();
        bookingsInDb.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateBookingAsync_BackToBackBookings_BothSucceed()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var firstBookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            BookedBy = "First User"
        };

        var secondBookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(2), // Starts when first booking ends
            EndTime = DateTime.Now.AddHours(3),
            BookedBy = "Second User"
        };

        // Act
        var firstResult = await _bookingService.CreateBookingAsync(firstBookingDto);
        var secondResult = await _bookingService.CreateBookingAsync(secondBookingDto);

        // Assert
        firstResult.Should().NotBeNull();
        secondResult.Should().NotBeNull();

        var bookingsInDb = await _context.Bookings.Where(b => b.RoomId == room.Id).ToListAsync();
        bookingsInDb.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateBookingAsync_DifferentRooms_BothSucceed()
    {
        // Arrange
        var room1 = new Room { Name = "Room 1", Capacity = 10 };
        var room2 = new Room { Name = "Room 2", Capacity = 15 };
        _context.Rooms.AddRange(room1, room2);
        await _context.SaveChangesAsync();

        var booking1Dto = new CreateBookingDTO
        {
            RoomId = room1.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2),
            BookedBy = "User 1"
        };

        var booking2Dto = new CreateBookingDTO
        {
            RoomId = room2.Id,
            StartTime = DateTime.Now.AddHours(1), // Same time as booking1
            EndTime = DateTime.Now.AddHours(2),
            BookedBy = "User 2"
        };

        // Act
        var result1 = await _bookingService.CreateBookingAsync(booking1Dto);
        var result2 = await _bookingService.CreateBookingAsync(booking2Dto);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();

        var bookings = await _context.Bookings.ToListAsync();
        bookings.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateBookingAsync_WithLongBookingPeriod_ReturnsBookingDto()
    {
        // Arrange
        var room = new Room { Name = "Test Room", Capacity = 10 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bookingDto = new CreateBookingDTO
        {
            RoomId = room.Id,
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(25),
            BookedBy = "Test User"
        };

        // Act
        var result = await _bookingService.CreateBookingAsync(bookingDto);

        // Assert
        result.Should().NotBeNull();
        result.StartTime.Should().Be(bookingDto.StartTime);
        result.EndTime.Should().Be(bookingDto.EndTime);

        var booking = await _context.Bookings.FirstOrDefaultAsync();
        booking.Should().NotBeNull();
        (booking!.EndTime - booking.StartTime).TotalHours.Should().BeApproximately(24, 1e-5);
    }
}
