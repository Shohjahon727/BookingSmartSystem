using System.Security.Claims;

namespace BookingSystem.API.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid GetUserId(this ClaimsPrincipal user)
		{
			var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? user.FindFirst("sub")?.Value;

			if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
				throw new UnauthorizedAccessException("Foydalanuvchi identifikatori topilmadi");

			return id;
		}
	}
}
