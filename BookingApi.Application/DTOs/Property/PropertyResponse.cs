using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.DTOs.Property
{
	public class PropertyResponse
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public decimal PricePerNight { get; set; }
		public int MaxGuests { get; set; }
		public int Bedrooms { get; set; }
		public int Bathrooms { get; set; }
		public string? ImageUrl { get; set; }
		public DateTime CreatedAt { get; set; }
		public string OwnerName { get; set; } = string.Empty;
	}
}
