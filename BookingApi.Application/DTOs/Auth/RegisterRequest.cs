using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.DTOs.Auth
{
	public class RegisterRequest
	{
		public string PhoneNumber { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
	}
}
