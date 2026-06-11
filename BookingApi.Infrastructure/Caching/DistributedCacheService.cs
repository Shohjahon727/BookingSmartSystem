using BookingApi.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookingApi.Infrastructure.Caching
{
	public class DistributedCacheService : ICacheService
	{
		private readonly IDistributedCache _cache;
		private readonly ILogger<DistributedCacheService> _logger;
		private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

		public DistributedCacheService(IDistributedCache cache, ILogger<DistributedCacheService> logger)
		{
			_cache = cache;
			_logger = logger;
		}

		public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			try
			{
				var data = await _cache.GetStringAsync(key, cancellationToken);
				return data == null ? default : JsonSerializer.Deserialize<T>(data, JsonOptions);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache read failed for key {Key}", key);
				return default;
			}
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
		{
			try
			{
				var options = new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
				};

				var data = JsonSerializer.Serialize(value, JsonOptions);
				await _cache.SetStringAsync(key, data, options, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache write failed for key {Key}", key);
			}
		}

		public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
		{
			try
			{
				await _cache.RemoveAsync(key, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Cache remove failed for key {Key}", key);
			}
		}
	}
}
