using BookingApi.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Services
{
	public class JwtService
	{
		private readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(User user)
		{
			var secret = _configuration["Jwt:Secret"]!;
			var issuer = _configuration["Jwt:Issuer"]!;
			var audience = _configuration["Jwt:Audience"]!;
			var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"]!);
			
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("phoneNumber", user.PhoneNumber),
				new Claim("fullName", user.FullName),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		public string GenerateRefreshToken()
		{
			return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
		}
	}
}
