using BookingApi.Application.DTOs.Property;
using BookingApi.Application.Exceptions;
using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;

namespace BookingApi.Infrastructure.Services
{
	public class PropertyService : IPropertyService
	{
		private readonly IPropertyRepository _propertyRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICacheService _cache;
		private readonly TimeSpan _cacheExpiration;

		public PropertyService(
			IPropertyRepository propertyRepository,
			IUnitOfWork unitOfWork,
			ICacheService cache,
			IConfiguration configuration)
		{
			_propertyRepository = propertyRepository;
			_unitOfWork = unitOfWork;
			_cache = cache;
			_cacheExpiration = TimeSpan.FromMinutes(configuration.GetValue("Redis:DefaultExpirationMinutes", 10));
		}

		public async Task<PropertyResponse> CreateAsync(CreatePropertyRequest request, Guid ownerId)
		{
			var property = new Property
			{
				Title = request.Title,
				Description = request.Description,
				Address = request.Address,
				City = request.City,
				Country = request.Country,
				PricePerNight = request.PricePerNight,
				MaxGuests = request.MaxGuests,
				MinGuests = 1,
				Bedrooms = request.Bedrooms,
				Bathrooms = request.Bathrooms,
				ImageUrl = request.ImageUrl,
				OwnerId = ownerId
			};

			await _propertyRepository.AddAsync(property);
			await _unitOfWork.SaveChangesAsync();

			var response = MapToResponse(property);
			await _cache.SetAsync(CacheKeys.Property(property.Id), response, _cacheExpiration);
			return response;
		}

		public async Task<PropertyResponse?> GetByIdAsync(Guid id)
		{
			var cacheKey = CacheKeys.Property(id);
			var cached = await _cache.GetAsync<PropertyResponse>(cacheKey);
			if (cached != null)
				return cached;

			var property = await _propertyRepository.GetByIdAsync(id);
			if (property == null)
				return null;

			var response = MapToResponse(property);
			await _cache.SetAsync(cacheKey, response, _cacheExpiration);
			return response;
		}

		public async Task<(IEnumerable<PropertyResponse> Items, int TotalCount)> SearchAsync(SearchPropertyRequest request)
		{
			var cacheKey = CacheKeys.PropertySearch(request);
			var cached = await _cache.GetAsync<SearchCacheResult>(cacheKey);
			if (cached != null)
				return (cached.Items, cached.TotalCount);

			var (items, totalCount) = await _propertyRepository.SearchPropertiesAsync(
				request.City,
				request.CheckInDate,
				request.CheckOutDate,
				request.MaxGuests,
				request.MinPrice,
				request.MaxPrice,
				request.Page,
				request.PageSize);

			var responses = items.Select(MapToResponse).ToList();
			await _cache.SetAsync(cacheKey, new SearchCacheResult(responses, totalCount), _cacheExpiration);
			return (responses, totalCount);
		}

		public async Task<IEnumerable<PropertyResponse>> GetMyPropertiesAsync(Guid ownerId)
		{
			var properties = await _propertyRepository.GetByOwnerIdAsync(ownerId);
			return properties.Select(MapToResponse);
		}

		public async Task<PropertyResponse> UpdateAsync(Guid id, CreatePropertyRequest request, Guid ownerId)
		{
			var property = await _propertyRepository.GetByIdAsync(id);
			if (property == null)
				throw new NotFoundException("Property", id);

			if (property.OwnerId != ownerId)
				throw new UnauthorizedAccessException("Bu uy sizga tegishli emas");

			property.Title = request.Title;
			property.Description = request.Description;
			property.Address = request.Address;
			property.City = request.City;
			property.Country = request.Country;
			property.PricePerNight = request.PricePerNight;
			property.MaxGuests = request.MaxGuests;
			property.Bedrooms = request.Bedrooms;
			property.Bathrooms = request.Bathrooms;
			property.ImageUrl = request.ImageUrl;

			_propertyRepository.UpdateAsync(property);
			await _unitOfWork.SaveChangesAsync();

			await _cache.RemoveAsync(CacheKeys.Property(id));
			return MapToResponse(property);
		}

		public async Task DeleteAsync(Guid id, Guid ownerId)
		{
			var property = await _propertyRepository.GetByIdAsync(id);
			if (property == null)
				throw new NotFoundException("Property", id);

			if (property.OwnerId != ownerId)
				throw new UnauthorizedAccessException("Bu uy sizga tegishli emas");

			_propertyRepository.DeleteAsync(property);
			await _unitOfWork.SaveChangesAsync();
			await _cache.RemoveAsync(CacheKeys.Property(id));
		}

		private sealed record SearchCacheResult(List<PropertyResponse> Items, int TotalCount);

		private static PropertyResponse MapToResponse(Property property)
		{
			return new PropertyResponse
			{
				Id = property.Id,
				Title = property.Title,
				Description = property.Description,
				Address = property.Address,
				City = property.City,
				Country = property.Country,
				PricePerNight = property.PricePerNight,
				MaxGuests = property.MaxGuests,
				Bedrooms = property.Bedrooms,
				Bathrooms = property.Bathrooms,
				ImageUrl = property.ImageUrl,
				CreatedAt = property.CreatedAt,
				OwnerName = property.Owner?.FullName ?? "Noma'lum"
			};
		}
	}
}
