using BookingApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IPropertyRepository
	{
		Task<Property?> GetByIdAsync(Guid id);
		Task<Property?> GetByWithBookingAsync(Guid id);
		// 🔥 Search + Filter + Pagination
		Task<(IEnumerable<Property> Items, int TotalCount)> SearchPropertiesAsync(string? city, DateTime? availableFrom, DateTime? availableTo,int? maxGuests,decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 10);

		Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
		Task<Property> AddAsync(Property property);
		void DeleteAsync(Property property);
		void UpdateAsync(Property property);
		Task<bool> ExisitingAsync(Guid id);
	}
}
