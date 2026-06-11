using BookingApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.DTOs.Booking
{
	public class BookingResponse
	{
		public Guid Id { get; set; }
		public Guid PropertyId { get; set; }
		public string PropertyTitle { get; set; } = string.Empty;
		public DateTime CheckInDate { get; set; }
		public DateTime CheckOutDate { get; set; }
		public int NumberOfNights { get; set; }
		public decimal TotalPrice { get; set; }
		public int GuestCount { get; set; }
		public BookingStatus Status { get; set; }
		public PaymentStatus PaymentStatus { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
