using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Services
{
	public static class PasswordHasher
	{
		public static string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
		}
		public static bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}
}
