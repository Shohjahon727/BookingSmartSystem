using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.DTOs.Booking
{
	public class CreateBookingRequest
	{
		public Guid PropertyId { get; set; }
		public DateTime CheckInDate { get; set; }
		public DateTime CheckOutDate { get; set; }
		public int GuestCount { get; set; }
	}
}
