using BookingApi.Application.DTOs.Booking;

namespace BookingApi.Application.Interfaces
{
	public interface IRealTimeNotificationService
	{
		Task NotifyBookingConfirmedAsync(Guid guestId, Guid hostId, BookingResponse booking, string propertyTitle);
		Task NotifyBookingCancelledAsync(Guid guestId, Guid hostId, BookingResponse booking, string? reason);
	}
}
