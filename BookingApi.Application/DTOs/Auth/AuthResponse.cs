using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.DTOs.Auth
{
	public class AuthResponse
	{
		public string Token { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
		public string PhoneNumber { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
	}
}
