using BookingApi.Application.Interfaces;
using BookingSystem.API.Hubs;
using BookingSystem.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BookingSystem.API.Extensions
{
	public static class SignalRExtensions
	{
		public static IServiceCollection AddSignalRServices(this IServiceCollection services)
		{
			services.AddSignalR();
			services.AddScoped<IRealTimeNotificationService, SignalRBookingNotificationService>();
			return services;
		}

		public static JwtBearerEvents CreateSignalRJwtEvents()
		{
			return new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];
					var path = context.HttpContext.Request.Path;

					if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
						context.Token = accessToken;

					return Task.CompletedTask;
				}
			};
		}
	}
}
