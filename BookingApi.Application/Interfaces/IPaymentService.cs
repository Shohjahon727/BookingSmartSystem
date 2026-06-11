using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IPaymentService
	{
		Task<(bool Success, string TransactionId, string Message)> ProcessPaymentAsync(Guid bookingId, decimal amount);
	}
}
