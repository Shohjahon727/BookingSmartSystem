using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.DTOs.Property
{
	public class SearchPropertyRequest
	{
		public string? City { get; set; }
		public DateTime? CheckInDate { get; set; }
		public DateTime? CheckOutDate { get; set; }
		public int? MaxGuests { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
