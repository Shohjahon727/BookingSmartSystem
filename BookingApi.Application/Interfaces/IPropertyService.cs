using BookingApi.Application.DTOs.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IPropertyService
	{
		Task<PropertyResponse> CreateAsync(CreatePropertyRequest request, Guid ownerId);
		Task<PropertyResponse?> GetByIdAsync(Guid id);
		Task<(IEnumerable<PropertyResponse> Items, int TotalCount)> SearchAsync(SearchPropertyRequest request);
		Task<IEnumerable<PropertyResponse>> GetMyPropertiesAsync(Guid ownerId);
		Task<PropertyResponse> UpdateAsync(Guid id, CreatePropertyRequest request, Guid ownerId);
		Task DeleteAsync(Guid id, Guid ownerId);
		Task<PropertyResponse> UploadImageAsync(Guid propertyId, Stream imageStream, string fileName, string contentType, long fileSize, Guid ownerId);
	}
}

