using BookingApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResponse> RegisterAsync(RegisterRequest request);
		Task<AuthResponse> LoginAsync(LoginRequest request);
	}
}
