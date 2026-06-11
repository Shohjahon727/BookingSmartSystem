namespace BookingApi.Application.Interfaces
{
	public interface INotificationService
	{
		Task SendEmailAsync(string to, string subject, string message, CancellationToken cancellationToken = default);
		Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
	}
}
