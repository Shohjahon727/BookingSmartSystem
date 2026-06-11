using BookingApi.Application.Interfaces;
using BookingApi.Application.Models;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.Notifications
{
	public class NotificationService : INotificationService
	{
		private readonly IEmailService _emailService;
		private readonly ILogger<NotificationService> _logger;

		public NotificationService(IEmailService emailService, ILogger<NotificationService> logger)
		{
			_emailService = emailService;
			_logger = logger;
		}

		public async Task SendEmailAsync(string to, string subject, string message, CancellationToken cancellationToken = default)
		{
			await _emailService.SendAsync(new EmailMessage
			{
				To = to,
				Subject = subject,
				HtmlBody = $"<p>{message}</p>",
				TextBody = message
			}, cancellationToken);
		}

		public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
		{
			_logger.LogInformation("[SMS] To: {Phone} | Message: {Message}", phoneNumber, message);
			return Task.CompletedTask;
		}
	}
}
