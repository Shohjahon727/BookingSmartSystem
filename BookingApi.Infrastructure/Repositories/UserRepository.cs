using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly BookingDbContext _context;

		public UserRepository(BookingDbContext context)
		{
			_context = context;
		}

		public async Task<User> AddAsync(User user)
		{
			await _context.Users.AddAsync(user);
			return user;
		}

		public void DeleteAsync(User user)
		{
			user.IsDeleted = true;
			user.UpdatedAt = DateTime.UtcNow;
			_context.Users.Update(user);
		}

		public async Task<bool> ExisitingAsync(Guid id)
		{
			return await _context.Users.AnyAsync(u => u.Id == id && !u.IsDeleted);
		}

		public async Task<User?> GetByIdAsync(Guid id)
		{
			return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
		}

		public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
		{
			return await _context.Users
			.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
		}

		public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
		{
			return await _context.Users
			.AnyAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
		}

		public void UpdateAsync(User user)
		{
			user.UpdatedAt = DateTime.UtcNow;
			_context.Users.Update(user);
		}
	}
}
