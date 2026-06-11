using BookingApi.Application.Interfaces;
using BookingApi.Infrastructure.Caching;

namespace BookingSystem.API.Extensions
{
	public static class CacheExtensions
	{
		public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
		{
			var redisEnabled = configuration.GetValue("Redis:Enabled", false);
			var redisConnection = configuration["Redis:ConnectionString"];

			if (redisEnabled && !string.IsNullOrWhiteSpace(redisConnection))
			{
				services.AddStackExchangeRedisCache(options =>
				{
					options.Configuration = redisConnection;
					options.InstanceName = configuration["Redis:InstanceName"] ?? "BookingApi:";
				});
			}
			else
			{
				services.AddDistributedMemoryCache();
			}

			services.AddSingleton<ICacheService, DistributedCacheService>();
			return services;
		}
	}
}
