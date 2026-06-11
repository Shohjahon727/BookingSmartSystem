using BookingApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id);
		Task<User?> GetByPhoneNumberAsync(string phoneNumber);
		Task<bool> PhoneNumberExistsAsync(string phoneNumber);
		Task<User> AddAsync(User user);
		void DeleteAsync(User user);
		void UpdateAsync(User user);
		Task<bool> ExisitingAsync(Guid id);
	}
}
