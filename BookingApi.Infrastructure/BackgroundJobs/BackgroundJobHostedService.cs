using BookingApi.Application.Interfaces;
using BookingApi.Application.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.BackgroundJobs
{
	public class BackgroundJobHostedService : BackgroundService
	{
		private readonly IBackgroundJobQueue _queue;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<BackgroundJobHostedService> _logger;

		public BackgroundJobHostedService(
			IBackgroundJobQueue queue,
			IServiceScopeFactory scopeFactory,
			ILogger<BackgroundJobHostedService> logger)
		{
			_queue = queue;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Background notification worker started");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var job = await _queue.DequeueAsync(stoppingToken);
					await ProcessJobAsync(job, stoppingToken);
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Background job processing error");
				}
			}

			_logger.LogInformation("Background notification worker stopped");
		}

		private async Task ProcessJobAsync(NotificationJob job, CancellationToken cancellationToken)
		{
			using var scope = _scopeFactory.CreateScope();
			var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

			switch (job.Channel)
			{
				case NotificationChannel.Email:
					await notificationService.SendEmailAsync(job.Recipient, job.Subject, job.Message, cancellationToken);
					break;
				case NotificationChannel.Sms:
					await notificationService.SendSmsAsync(job.Recipient, job.Message, cancellationToken);
					break;
			}
		}
	}
}
