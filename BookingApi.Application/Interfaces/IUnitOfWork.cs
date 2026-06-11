using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IUnitOfWork
	{
		Task<int> SaveChangesAsync();
		Task<IDisposable> BeginTransactionAsync();
		Task CommitAsync();
		Task RollbackAsync();
	}
}
