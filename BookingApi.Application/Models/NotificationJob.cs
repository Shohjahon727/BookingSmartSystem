namespace BookingApi.Application.Models
{
	public enum NotificationChannel
	{
		Email,
		Sms
	}

	public class NotificationJob
	{
		public NotificationChannel Channel { get; set; }
		public string Recipient { get; set; } = string.Empty;
		public string Subject { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
	}
}
