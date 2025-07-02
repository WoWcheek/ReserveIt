using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReserveIt.BLL.DTOs;
using ReserveIt.BLL.Interfaces;

namespace ReserveIt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDTO>> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        return booking is null
            ? NotFound()
            : Ok(booking);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDTO>> CreateBooking(CreateBookingDTO BookingDTO)
    {
        var (isSuccess, errorMessage, booking) = await _bookingService.CreateBookingAsync(BookingDTO);

        return isSuccess
            ? CreatedAtAction(nameof(GetBooking), new { id = booking!.Id }, booking)
            : BadRequest(errorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBooking(int id)
    {
        var isDeleted = await _bookingService.DeleteBookingAsync(id);
        return isDeleted
            ? NoContent()
            : NotFound();
    }
}
