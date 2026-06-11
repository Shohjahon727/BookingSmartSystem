using BookingApi.Application.Interfaces;
using BookingApi.Infrastructure.Search;

namespace BookingSystem.API.Extensions
{
	public static class ElasticsearchExtensions
	{
		public static IServiceCollection AddElasticsearchServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IPropertySearchService, ElasticsearchPropertySearchService>();

			if (configuration.GetValue("Elasticsearch:Enabled", false))
				services.AddHostedService<ElasticsearchIndexHostedService>();

			return services;
		}
	}
}
