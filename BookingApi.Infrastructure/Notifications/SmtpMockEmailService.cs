using BookingApi.Application.DTOs.Booking;
using BookingApi.Application.Interfaces;
using BookingApi.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BookingApi.Infrastructure.Notifications
{
	public class SmtpMockEmailService : IEmailService
	{
		private readonly ILogger<SmtpMockEmailService> _logger;
		private readonly string _fromAddress;
		private readonly string _smtpHost;
		private readonly int _smtpPort;

		public SmtpMockEmailService(IConfiguration configuration, ILogger<SmtpMockEmailService> logger)
		{
			_logger = logger;
			_fromAddress = configuration["Smtp:FromAddress"] ?? "noreply@bookingsmart.uz";
			_smtpHost = configuration["Smtp:Host"] ?? "smtp.mock.local";
			_smtpPort = configuration.GetValue("Smtp:Port", 587);
		}

		public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
		{
			var sb = new StringBuilder();
			sb.AppendLine("========== SMTP MOCK ==========");
			sb.AppendLine($"CONNECT {_smtpHost}:{_smtpPort}");
			sb.AppendLine($"MAIL FROM:<{_fromAddress}>");
			sb.AppendLine($"RCPT TO:<{message.To}>");
			sb.AppendLine($"SUBJECT: {message.Subject}");
			sb.AppendLine("--- HTML BODY ---");
			sb.AppendLine(message.HtmlBody);
			if (!string.IsNullOrWhiteSpace(message.TextBody))
			{
				sb.AppendLine("--- TEXT BODY ---");
				sb.AppendLine(message.TextBody);
			}
			sb.AppendLine("========== SENT OK ==========");

			_logger.LogInformation("{SmtpLog}", sb.ToString());
			return Task.CompletedTask;
		}

		public Task SendBookingConfirmationAsync(
			string to,
			string guestName,
			BookingResponse booking,
			string propertyTitle,
			CancellationToken cancellationToken = default)
		{
			var html = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif;'>
  <h2 style='color: #2563eb;'>Bron tasdiqlandi!</h2>
  <p>Hurmatli <strong>{guestName}</strong>,</p>
  <p><strong>{propertyTitle}</strong> uchun broningiz muvaffaqiyatli tasdiqlandi.</p>
  <table style='border-collapse: collapse;'>
    <tr><td style='padding: 6px;'>Bron ID:</td><td><strong>{booking.Id}</strong></td></tr>
    <tr><td style='padding: 6px;'>Kirish:</td><td>{booking.CheckInDate:dd.MM.yyyy}</td></tr>
    <tr><td style='padding: 6px;'>Chiqish:</td><td>{booking.CheckOutDate:dd.MM.yyyy}</td></tr>
    <tr><td style='padding: 6px;'>Kechalar:</td><td>{booking.NumberOfNights}</td></tr>
    <tr><td style='padding: 6px;'>Mehmonlar:</td><td>{booking.GuestCount}</td></tr>
    <tr><td style='padding: 6px;'>Jami:</td><td><strong>{booking.TotalPrice:N0} so'm</strong></td></tr>
  </table>
  <p style='color: #666;'>Booking Smart System</p>
</body>
</html>";

			var text = $"Bron tasdiqlandi: {propertyTitle}, {booking.CheckInDate:dd.MM.yyyy} - {booking.CheckOutDate:dd.MM.yyyy}, {booking.TotalPrice:N0} so'm";

			return SendAsync(new EmailMessage
			{
				To = to,
				Subject = $"Bron tasdiqlandi — {propertyTitle}",
				HtmlBody = html,
				TextBody = text
			}, cancellationToken);
		}
	}
}
