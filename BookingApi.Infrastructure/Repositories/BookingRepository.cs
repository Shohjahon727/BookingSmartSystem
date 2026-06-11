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
	public class BookingRepository : IBookingRepository
	{
		private readonly BookingDbContext _context;

		public BookingRepository(BookingDbContext context)
		{
			_context = context;
		}
		public async Task<Booking> AddAsync(Booking booking)
		{
			await _context.Bookings.AddAsync(booking);
			return booking;
		}

		public void DeleteAsync(Booking booking)
		{
			booking.IsDeleted = true;
			booking.UpdatedAt = DateTime.UtcNow;
			_context.Bookings.Update(booking);
		}

		public async Task<IEnumerable<DateTime>> GetBookedDateAsync(Guid propertyId, DateTime form, DateTime to)
		{
			var bookings = await _context.Bookings
			.Where(b => b.PropertyId == propertyId)
			.Where(b => !b.IsDeleted)
			.Where(b => b.Status != BookingStatus.Cancelled)
			.Where(b => b.CheckInDate < to && b.CheckOutDate > form)
			.ToListAsync();
			var bookedDates = new List<DateTime>();
			foreach (var booking in bookings)
			{
				for (var date = booking.CheckInDate; date < booking.CheckOutDate; date = date.AddDays(1))
				{
					if (date >= form && date <= to)
						bookedDates.Add(date);
				}
			}

			return bookedDates.Distinct().OrderBy(d => d);
		}

		public async Task<Booking?> GetByIdAsync(Guid id)
		{
			return await _context.Bookings
			.AsNoTracking()  
			.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
		}

		public async Task<Booking?> GetByIdWithDetailsAsync(Guid id)
		{
			return await _context.Bookings
			.Include(b => b.Property)      
			.Include(b => b.Guest)
			.Include(b => b.Payment)
			.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
		}

		public async Task<IEnumerable<Booking>> GetPropertyBookingAsync(Guid propertyId)
		{
			return await _context.Bookings
			.AsNoTracking()
			.Where(b => b.PropertyId == propertyId && !b.IsDeleted)
			.ToListAsync();
		}

		public async Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId)
		{
			return await _context.Bookings
			.AsNoTracking()
			.Where(b => b.GuestId == userId && !b.IsDeleted)
			.Include(b => b.Property)
			.OrderByDescending(b => b.CreatedAt)
			.ToListAsync();
		}

		public async Task<bool> IsPropertyAvailableAsync(Guid propertyId, DateTime checkIn, DateTime checkOut)
		{
			var hasOverlap = await _context.Bookings
			.Where(b => b.PropertyId == propertyId)
			.Where(b => !b.IsDeleted)
			.Where(b => b.Status != BookingStatus.Cancelled)  
			.Where(b =>
				checkIn < b.CheckOutDate &&    
				checkOut > b.CheckInDate)     
			.AnyAsync();

			return !hasOverlap;
		}

		public void UpdateAsync(Booking booking)
		{
			booking.UpdatedAt = DateTime.UtcNow;
			_context.Bookings.Update(booking);
		}
	}
}
