using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Domain.Enums;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Repositories
{
	public class PropertyRepository : IPropertyRepository
	{
		private readonly BookingDbContext _context;

		public PropertyRepository(BookingDbContext context)
		{
			_context = context;
		}

		public async Task<Property> AddAsync(Property property)
		{
			await _context.Properties.AddAsync(property);
			return property;
		}

		public void DeleteAsync(Property property)
		{
			property.IsDeleted = true;
			property.UpdatedAt = DateTime.UtcNow;
			_context.Properties.Update(property);
		}

		public async Task<bool> ExisitingAsync(Guid id)
		{
			return await _context.Properties.AnyAsync(p => p.Id == id && !p.IsDeleted);
		}

		public async Task<Property?> GetByIdAsync(Guid id)
		{
			return await _context.Properties
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
		}

		public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
		{
			return await _context.Properties
				.AsNoTracking()
				.Where(p => p.OwnerId == ownerId && !p.IsDeleted)
				.OrderByDescending(p => p.CreatedAt)
				.ToListAsync();
		}

		public async Task<Property?> GetByWithBookingAsync(Guid id)
		{
			return await _context.Properties
			.Include(p => p.Bookings.Where(b => !b.IsDeleted && b.Status != BookingStatus.Cancelled))
			.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
		}

		public async Task<(IEnumerable<Property> Items, int TotalCount)> SearchPropertiesAsync(string? city, DateTime? availableFrom, DateTime? availableTo, int? maxGuests, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 10)
		{
			// IQueryable build qilamiz (deferred execution)
			var query = _context.Properties
				.AsNoTracking()
				.Where(p => !p.IsDeleted)
				.AsQueryable();

			// Filter: City
			if (!string.IsNullOrWhiteSpace(city))
				query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));

			// Filter: Price range
			if (minPrice.HasValue)
				query = query.Where(p => p.PricePerNight >= minPrice.Value);
			if (maxPrice.HasValue)
				query = query.Where(p => p.PricePerNight <= maxPrice.Value);

			// Filter: Max guests
			if (maxGuests.HasValue)
				query = query.Where(p => p.MaxGuests >= maxGuests.Value);

			// 🔥 Filter: Availability (complex subquery)
			if (availableFrom.HasValue && availableTo.HasValue)
			{
				query = query.Where(p => !_context.Bookings
					.Any(b => b.PropertyId == p.Id &&
							 !b.IsDeleted &&
							 b.Status != BookingStatus.Cancelled &&
							 availableFrom.Value < b.CheckOutDate &&
							 availableTo.Value > b.CheckInDate));
			}

			// Total count (before pagination)
			var totalCount = await query.CountAsync();

			// Pagination + Sorting
			var items = await query
				.OrderByDescending(p => p.CreatedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public void UpdateAsync(Property property)
		{
			property.UpdatedAt = DateTime.UtcNow;
			_context.Properties.Update(property);
		}
	}
}
