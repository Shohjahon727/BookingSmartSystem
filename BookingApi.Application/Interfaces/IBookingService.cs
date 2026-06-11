using BookingApi.Application.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IBookingService
	{
		Task<BookingResponse> CreateAsync(CreateBookingRequest request, Guid guestId);
		Task<BookingResponse?> GetByIdAsync(Guid id, Guid guestId);
		Task<IEnumerable<BookingResponse>> GetMyBookingsAsync(Guid guestId);
		Task CancelAsync(Guid id, Guid guestId, string? reason = null);
	}
}
