using BookingApi.Application.Interfaces;
using BookingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly BookingDbContext _context;
		private IDbContextTransaction? _transaction;

		public UnitOfWork(BookingDbContext context)
		{
			_context = context;
		}
		public async Task<IDisposable> BeginTransactionAsync()
		{
			_transaction = await _context.Database.BeginTransactionAsync();
			return _transaction;
		}

		public async Task CommitAsync()
		{
			if (_transaction != null)
			{
				await _transaction.CommitAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async Task RollbackAsync()
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
