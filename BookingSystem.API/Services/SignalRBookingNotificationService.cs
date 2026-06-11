using BookingApi.Application.DTOs.Booking;
using BookingApi.Application.Interfaces;
using BookingSystem.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BookingSystem.API.Services
{
	public class SignalRBookingNotificationService : IRealTimeNotificationService
	{
		private readonly IHubContext<BookingNotificationHub> _hubContext;
		private readonly ILogger<SignalRBookingNotificationService> _logger;

		public SignalRBookingNotificationService(
			IHubContext<BookingNotificationHub> hubContext,
			ILogger<SignalRBookingNotificationService> logger)
		{
			_hubContext = hubContext;
			_logger = logger;
		}

		public async Task NotifyBookingConfirmedAsync(Guid guestId, Guid hostId, BookingResponse booking, string propertyTitle)
		{
			var payload = new
			{
				type = "booking_confirmed",
				booking.Id,
				booking.PropertyId,
				propertyTitle,
				booking.CheckInDate,
				booking.CheckOutDate,
				booking.TotalPrice,
				booking.Status,
				message = $"Bron tasdiqlandi: {propertyTitle}"
			};

			await _hubContext.Clients.Group($"user-{guestId}").SendAsync("BookingNotification", payload);
			await _hubContext.Clients.Group($"user-{hostId}").SendAsync("BookingNotification", new
			{
				type = "new_booking",
				booking.Id,
				booking.PropertyId,
				propertyTitle,
				booking.CheckInDate,
				booking.CheckOutDate,
				booking.TotalPrice,
				message = $"Yangi bron: {propertyTitle}"
			});

			_logger.LogInformation("SignalR: booking confirmed sent to guest {GuestId} and host {HostId}", guestId, hostId);
		}

		public async Task NotifyBookingCancelledAsync(Guid guestId, Guid hostId, BookingResponse booking, string? reason)
		{
			var payload = new
			{
				type = "booking_cancelled",
				booking.Id,
				booking.PropertyId,
				reason,
				message = $"Bron bekor qilindi: {reason ?? "Sabab ko'rsatilmagan"}"
			};

			await _hubContext.Clients.Group($"user-{guestId}").SendAsync("BookingNotification", payload);
			await _hubContext.Clients.Group($"user-{hostId}").SendAsync("BookingNotification", payload);

			_logger.LogInformation("SignalR: booking cancelled sent to guest {GuestId} and host {HostId}", guestId, hostId);
		}
	}
}
