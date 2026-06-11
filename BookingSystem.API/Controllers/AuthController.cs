using Asp.Versioning;
using BookingApi.Application.DTOs.Auth;
using BookingApi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BookingSystem.API.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		[EnableRateLimiting("auth")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var response = await _authService.RegisterAsync(request);
			return Ok(response);
		}

		[HttpPost("login")]
		[EnableRateLimiting("auth")]
		public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
		{
			var response = await _authService.LoginAsync(request);
			return Ok(response);
		}
	}
}
