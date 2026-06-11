using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace BookingSystem.API.Extensions
{
	public static class HealthCheckResponseWriter
	{
		public static Task WriteAsync(HttpContext context, HealthReport report)
		{
			context.Response.ContentType = "application/json";

			var response = new
			{
				status = report.Status.ToString(),
				totalDuration = report.TotalDuration.TotalMilliseconds,
				checks = report.Entries.Select(e => new
				{
					name = e.Key,
					status = e.Value.Status.ToString(),
					duration = e.Value.Duration.TotalMilliseconds,
					description = e.Value.Description,
					error = e.Value.Exception?.Message
				})
			};

			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
		}
	}
}
