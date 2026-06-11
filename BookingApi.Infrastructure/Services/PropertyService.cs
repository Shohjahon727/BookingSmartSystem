using BookingApi.Application.DTOs.Property;
using BookingApi.Application.Exceptions;
using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Services
{
	public class PropertyService : IPropertyService
	{
		private readonly IPropertyRepository _propertyRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PropertyService(IPropertyRepository propertyRepository, IUnitOfWork unitOfWork)
		{
			_propertyRepository = propertyRepository;
			_unitOfWork = unitOfWork;
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

			return MapToResponse(property);
		}

		public async Task<PropertyResponse?> GetByIdAsync(Guid id)
		{
			var property = await _propertyRepository.GetByIdAsync(id);
			return property == null ? null : MapToResponse(property);
		}

		public async Task<(IEnumerable<PropertyResponse> Items, int TotalCount)> SearchAsync(SearchPropertyRequest request)
		{
			var (items, totalCount) = await _propertyRepository.SearchPropertiesAsync(
				request.City,
				request.CheckInDate,
				request.CheckOutDate,
				request.MaxGuests,
				request.MinPrice,
				request.MaxPrice,
				request.Page,
				request.PageSize);

			var responses = items.Select(MapToResponse);
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
		}

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
