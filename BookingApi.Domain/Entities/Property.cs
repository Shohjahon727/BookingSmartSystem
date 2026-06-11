using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Entities
{
	public class Property : BaseEntity
	{
		public string Tilte { get; set; }
		public string Description { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public decimal PricePerNight { get; set; }
		public int MaxGuests { get; set; }
		public int MinGuests { get; set; }
		public string BadRooms { get; set; }
		public string BathRooms { get; set; }
		public string? ImageUrl { get; set; }

		public Guid OwnerId { get; set; }
		public User Owner { get; set; } = null!;

		public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
	}
}
