using BookingApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Entities
{
	public class Payment : BaseEntity
	{
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; } = string.Empty;
		public string TransactionId { get; set; } = Guid.NewGuid().ToString();
		public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
		public DateTime? ProcessedAt { get; set; }

		public Guid BookingId { get; set; }
		public Booking Booking { get; set; } = null!;
	}
}
