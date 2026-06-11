using BookingApi.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly ILogger<PaymentService> _logger;
		private static readonly Random _random = new();

		public PaymentService(ILogger<PaymentService> logger)
		{
			_logger = logger;
		}

		public async Task<(bool Success, string TransactionId, string Message)> ProcessPaymentAsync(
			Guid bookingId,
			decimal amount)
		{
			// 🎭 MOCK PAYMENT - Real payment gateway o'rniga
			_logger.LogInformation("Processing mock payment for booking {BookingId}, amount: {Amount}",
				bookingId, amount);

			// Simulate network delay
			await Task.Delay(500);

			// 🎲 90% success rate (test uchun)
			var isSuccess = _random.Next(1, 101) <= 90;

			var transactionId = Guid.NewGuid().ToString("N")[..16].ToUpper();

			if (isSuccess)
			{
				_logger.LogInformation("Payment successful. TransactionId: {TransactionId}", transactionId);
				return (true, transactionId, "To'lov muvaffaqiyatli amalga oshirildi");
			}
			else
			{
				_logger.LogWarning("Payment failed for booking {BookingId}", bookingId);
				return (false, transactionId, "Karta ma'lumotlari noto'g'ri yoki mablag' yetarli emas");
			}
		}
	}

}
