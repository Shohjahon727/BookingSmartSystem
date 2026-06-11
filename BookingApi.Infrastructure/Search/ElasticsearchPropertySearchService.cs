using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Data;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.Search
{
	public class ElasticsearchPropertySearchService : IPropertySearchService
	{
		private readonly ElasticsearchClient _client;
		private readonly IPropertyRepository _propertyRepository;
		private readonly BookingDbContext _dbContext;
		private readonly ILogger<ElasticsearchPropertySearchService> _logger;
		private readonly string _indexName;
		private readonly bool _enabled;

		public bool IsEnabled => _enabled;

		public ElasticsearchPropertySearchService(
			IConfiguration configuration,
			IPropertyRepository propertyRepository,
			BookingDbContext dbContext,
			ILogger<ElasticsearchPropertySearchService> logger)
		{
			_propertyRepository = propertyRepository;
			_dbContext = dbContext;
			_logger = logger;
			_enabled = configuration.GetValue("Elasticsearch:Enabled", false);
			_indexName = configuration["Elasticsearch:IndexName"] ?? "properties";

			var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
			var settings = new ElasticsearchClientSettings(new Uri(url))
				.DefaultIndex(_indexName);
			_client = new ElasticsearchClient(settings);
		}

		public async Task<(IEnumerable<Property> Items, int TotalCount)> SearchAsync(
			string? city,
			DateTime? availableFrom,
			DateTime? availableTo,
			int? maxGuests,
			decimal? minPrice,
			decimal? maxPrice,
			int page = 1,
			int pageSize = 10,
			CancellationToken cancellationToken = default)
		{
			// Availability overlap — SQL da aniqroq
			if (!_enabled || (availableFrom.HasValue && availableTo.HasValue))
				return await _propertyRepository.SearchPropertiesAsync(city, availableFrom, availableTo, maxGuests, minPrice, maxPrice, page, pageSize);

			try
			{
				var mustQueries = new List<Query>
				{
					new TermQuery(new Field("isDeleted")) { Value = false }
				};

				if (!string.IsNullOrWhiteSpace(city))
				{
					mustQueries.Add(new MatchQuery(new Field("city"))
					{
						Query = city,
						Fuzziness = new Fuzziness("AUTO")
					});
				}

				if (minPrice.HasValue || maxPrice.HasValue)
				{
					mustQueries.Add(new NumberRangeQuery(new Field("pricePerNight"))
					{
						Gte = minPrice.HasValue ? (double)minPrice.Value : null,
						Lte = maxPrice.HasValue ? (double)maxPrice.Value : null
					});
				}

				if (maxGuests.HasValue)
				{
					mustQueries.Add(new NumberRangeQuery(new Field("maxGuests"))
					{
						Gte = maxGuests.Value
					});
				}

				var response = await _client.SearchAsync<PropertySearchDocument>(s => s
					.Indices(_indexName)
					.From((page - 1) * pageSize)
					.Size(pageSize)
					.Sort(sort => sort.Field(new Field("createdAt"), new FieldSort { Order = SortOrder.Desc }))
					.Query(q => q.Bool(b => b.Must(mustQueries.ToArray()))),
					cancellationToken);

				if (!response.IsValidResponse)
				{
					_logger.LogWarning("Elasticsearch search failed, falling back to SQL: {Error}", response.ElasticsearchServerError);
					return await _propertyRepository.SearchPropertiesAsync(city, availableFrom, availableTo, maxGuests, minPrice, maxPrice, page, pageSize);
				}

				var docs = response.Documents.ToList();
				var properties = docs.Select(MapToEntity).ToList();
				var total = response.Total;
				return (properties, (int)total);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Elasticsearch unavailable, falling back to SQL");
				return await _propertyRepository.SearchPropertiesAsync(city, availableFrom, availableTo, maxGuests, minPrice, maxPrice, page, pageSize);
			}
		}

		public async Task IndexPropertyAsync(Property property, CancellationToken cancellationToken = default)
		{
			if (!_enabled) return;

			try
			{
				await EnsureIndexAsync(cancellationToken);
				var doc = MapToDocument(property);
				await _client.IndexAsync(doc, i => i.Index(_indexName).Id(property.Id.ToString()), cancellationToken);
				_logger.LogDebug("Indexed property {Id} in Elasticsearch", property.Id);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Failed to index property {Id}", property.Id);
			}
		}

		public async Task DeleteFromIndexAsync(Guid propertyId, CancellationToken cancellationToken = default)
		{
			if (!_enabled) return;

			try
			{
				await _client.DeleteAsync(new DeleteRequest(_indexName, propertyId.ToString()), cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Failed to delete property {Id} from index", propertyId);
			}
		}

		public async Task ReindexAllAsync(CancellationToken cancellationToken = default)
		{
			if (!_enabled) return;

			await EnsureIndexAsync(cancellationToken);

			var properties = await _dbContext.Properties
				.AsNoTracking()
				.Where(p => !p.IsDeleted)
				.ToListAsync(cancellationToken);

			foreach (var property in properties)
				await IndexPropertyAsync(property, cancellationToken);

			_logger.LogInformation("Reindexed {Count} properties to Elasticsearch", properties.Count);
		}

		private async Task EnsureIndexAsync(CancellationToken cancellationToken)
		{
			var exists = await _client.Indices.ExistsAsync(_indexName, cancellationToken);
			if (exists.Exists) return;

			await _client.Indices.CreateAsync(_indexName, c => c
				.Mappings(m => m.Properties<PropertySearchDocument>(p => p
					.Keyword(k => k.Id)
					.Text(t => t.Title)
					.Text(t => t.Description)
					.Text(t => t.City)
					.Text(t => t.Country))),
				cancellationToken);
		}

		private static PropertySearchDocument MapToDocument(Property p) => new()
		{
			Id = p.Id,
			Title = p.Title,
			Description = p.Description,
			Address = p.Address,
			City = p.City,
			Country = p.Country,
			PricePerNight = p.PricePerNight,
			MaxGuests = p.MaxGuests,
			Bedrooms = p.Bedrooms,
			Bathrooms = p.Bathrooms,
			ImageUrl = p.ImageUrl,
			OwnerId = p.OwnerId,
			CreatedAt = p.CreatedAt,
			IsDeleted = p.IsDeleted
		};

		private static Property MapToEntity(PropertySearchDocument d) => new()
		{
			Id = d.Id,
			Title = d.Title,
			Description = d.Description,
			Address = d.Address,
			City = d.City,
			Country = d.Country,
			PricePerNight = d.PricePerNight,
			MaxGuests = d.MaxGuests,
			Bedrooms = d.Bedrooms,
			Bathrooms = d.Bathrooms,
			ImageUrl = d.ImageUrl,
			OwnerId = d.OwnerId,
			CreatedAt = d.CreatedAt,
			IsDeleted = d.IsDeleted
		};
	}
}
