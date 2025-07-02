using ReserveIt.BLL.DTOs;

namespace ReserveIt.BLL.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
    Task<BookingDTO> GetBookingByIdAsync(int id);
    Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDto);
    Task<bool> DeleteBookingAsync(int id);
}
