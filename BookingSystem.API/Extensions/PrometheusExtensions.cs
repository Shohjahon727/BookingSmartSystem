using Prometheus;

namespace BookingSystem.API.Extensions
{
	public static class PrometheusExtensions
	{
		public static IServiceCollection AddPrometheusMetrics(this IServiceCollection services)
		{
			return services;
		}

		public static WebApplication UsePrometheusMetrics(this WebApplication app)
		{
			app.UseHttpMetrics();
			app.MapMetrics("/metrics");
			return app;
		}
	}
}
