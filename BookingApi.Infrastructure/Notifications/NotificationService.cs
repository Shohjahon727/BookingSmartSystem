using BookingApi.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.Notifications
{
	public class NotificationService : INotificationService
	{
		private readonly ILogger<NotificationService> _logger;

		public NotificationService(ILogger<NotificationService> logger)
		{
			_logger = logger;
		}

		public Task SendEmailAsync(string to, string subject, string message, CancellationToken cancellationToken = default)
		{
			// Mock email — production da SendGrid, SMTP va h.k. ulanadi
			_logger.LogInformation(
				"[EMAIL] To: {To} | Subject: {Subject} | Message: {Message}",
				to, subject, message);

			return Task.CompletedTask;
		}

		public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
		{
			// Mock SMS — production da Eskiz, Twilio va h.k. ulanadi
			_logger.LogInformation(
				"[SMS] To: {Phone} | Message: {Message}",
				phoneNumber, message);

			return Task.CompletedTask;
		}
	}
}
