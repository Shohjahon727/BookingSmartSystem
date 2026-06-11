using BookingApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Entities
{
	public class Booking : BaseEntity
	{
		public DateTime ChekInDate { get; set; }
		public DateTime CheckOutDate { get; set; }

		public int NumberOfNights { get; set; }
		public decimal TotalPrice { get; set; }
		public int GuestCount { get; set; }

		public BookingStatus Status { get; set; } = BookingStatus.Pending;
		public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

		public string? CancellationReason { get; set; }

		public Guid PropertyId { get; set; }
		public Property Property { get; set; } = null!;

		public Guid GuestId { get; set; }
		public User Guest { get; set; } = null!;

		public Payment? Payment { get; set; }
	}
}
