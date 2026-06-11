using BookingApi.Application.DTOs.Booking;
using BookingApi.Application.Models;

namespace BookingApi.Application.Interfaces
{
	public interface IEmailService
	{
		Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
		Task SendBookingConfirmationAsync(string to, string guestName, BookingResponse booking, string propertyTitle, CancellationToken cancellationToken = default);
	}
}
