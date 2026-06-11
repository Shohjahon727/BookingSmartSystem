using BookingApi.Application.Interfaces;
using BookingApi.Infrastructure.BackgroundJobs;
using BookingApi.Infrastructure.Notifications;

namespace BookingSystem.API.Extensions
{
	public static class BackgroundJobExtensions
	{
		public static IServiceCollection AddBackgroundJobServices(this IServiceCollection services)
		{
			services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddHostedService<BackgroundJobHostedService>();
			return services;
		}
	}
}
