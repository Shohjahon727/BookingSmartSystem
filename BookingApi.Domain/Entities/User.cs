using BookingApi.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Entities
{
	public class User : BaseEntity
	{
		public string PhoneNumber { get; set; } 
		public string FullName { get; set; }
		public string PasswordHash { get; set; }
		public UserRole Role { get; set; } = UserRole.Guest;

		public ICollection<Property> Properties { get; set; } = new List<Property>();
		public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
	}
}
