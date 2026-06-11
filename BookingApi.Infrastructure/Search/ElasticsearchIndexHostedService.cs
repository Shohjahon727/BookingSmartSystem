using BookingApi.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.Search
{
	public class ElasticsearchIndexHostedService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<ElasticsearchIndexHostedService> _logger;

		public ElasticsearchIndexHostedService(IServiceScopeFactory scopeFactory, ILogger<ElasticsearchIndexHostedService> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(3000, stoppingToken);

			using var scope = _scopeFactory.CreateScope();
			var searchService = scope.ServiceProvider.GetRequiredService<IPropertySearchService>();

			if (!searchService.IsEnabled)
			{
				_logger.LogInformation("Elasticsearch disabled — skipping reindex");
				return;
			}

			try
			{
				await searchService.ReindexAllAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Elasticsearch reindex on startup failed");
			}
		}
	}
}
