using BookingApi.Infrastructure.Data;

namespace BookingSystem.API.Extensions
{
	public static class HealthCheckExtensions
	{
		public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
		{
			var healthChecks = services.AddHealthChecks()
				.AddDbContextCheck<BookingDbContext>("database", tags: new[] { "ready", "db" })
				.AddNpgSql(
					configuration.GetConnectionString("DefaultConnection")!,
					name: "postgresql",
					tags: new[] { "ready", "db" });

			var redisEnabled = configuration.GetValue("Redis:Enabled", false);
			var redisConnection = configuration["Redis:ConnectionString"];

			if (redisEnabled && !string.IsNullOrWhiteSpace(redisConnection))
			{
				healthChecks.AddRedis(redisConnection, name: "redis", tags: new[] { "ready", "cache" });
			}

			return services;
		}

		public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
		{
			app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				Predicate = _ => true,
				ResponseWriter = HealthCheckResponseWriter.WriteAsync
			});

			app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				Predicate = check => check.Tags.Contains("ready"),
				ResponseWriter = HealthCheckResponseWriter.WriteAsync
			});

			app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				Predicate = _ => false,
				ResponseWriter = HealthCheckResponseWriter.WriteAsync
			});

			return app;
		}
	}
}
