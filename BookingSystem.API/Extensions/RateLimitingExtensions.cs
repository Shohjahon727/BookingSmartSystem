using System.Threading.RateLimiting;

namespace BookingSystem.API.Extensions
{
	public static class RateLimitingExtensions
	{
		public static IServiceCollection AddRateLimitingServices(this IServiceCollection services, IConfiguration configuration)
		{
			var permitLimit = configuration.GetValue("RateLimiting:PermitLimit", 100);
			var windowSeconds = configuration.GetValue("RateLimiting:WindowSeconds", 60);
			var authPermitLimit = configuration.GetValue("RateLimiting:AuthPermitLimit", 10);

			services.AddRateLimiter(options =>
			{
				options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

				options.OnRejected = async (context, token) =>
				{
					context.HttpContext.Response.ContentType = "application/json";
					await context.HttpContext.Response.WriteAsJsonAsync(new
					{
						statusCode = 429,
						message = "Juda ko'p so'rov yuborildi. Biroz kutib qayta urinib ko'ring."
					}, token);
				};

				options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
				{
					var partitionKey = context.User.Identity?.IsAuthenticated == true
						? context.User.FindFirst("sub")?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous"
						: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

					return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = permitLimit,
						Window = TimeSpan.FromSeconds(windowSeconds),
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 0
					});
				});

				options.AddPolicy("auth", context =>
				{
					var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

					return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = authPermitLimit,
						Window = TimeSpan.FromSeconds(windowSeconds),
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 0
					});
				});
			});

			return services;
		}
	}
}
