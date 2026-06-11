namespace BookingApi.Application.Models
{
	public enum NotificationChannel
	{
		Email,
		Sms,
		BookingConfirmationEmail
	}

	public class NotificationJob
	{
		public NotificationChannel Channel { get; set; }
		public string Recipient { get; set; } = string.Empty;
		public string Subject { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string? GuestName { get; set; }
		public string? PropertyTitle { get; set; }
		public string? BookingDataJson { get; set; }
	}
}
