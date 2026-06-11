using BookingApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IBookingRepository
	{
		Task<Booking?> GetByIdAsync(Guid id);
		Task<Booking> GetByIdWithDetailsAsync(Guid id);
		Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId);
		Task<IEnumerable<Booking>> GetPropertyBookingAsync(Guid propertyId);

		// 🔥 ENG MUHIM: Date Overlap Check
		Task<bool> IsPropertyAvailableAsycn(Guid propertyId, DateTime checkIn, DateTime checkOut);
		// 🔥 Complex query: Property band sanalari
		Task<IEnumerable<DateTime>> GetBookedDateAsync(Guid propertyId,DateTime form, DateTime to);

		Task<Booking> AddAsync(Booking booking);
		void DeleteAsync(Booking booking);
		void UpdateAsync(Booking booking);
	}
}
