using BookingApi.Application.DTOs.Property;
using System.Text;

namespace BookingApi.Infrastructure.Caching
{
	public static class CacheKeys
	{
		public static string Property(Guid id) => $"property:{id}";

		public static string PropertySearch(SearchPropertyRequest request)
		{
			var sb = new StringBuilder("property:search:");
			sb.Append(request.City ?? "all").Append(':');
			sb.Append(request.CheckInDate?.ToString("O") ?? "-").Append(':');
			sb.Append(request.CheckOutDate?.ToString("O") ?? "-").Append(':');
			sb.Append(request.MaxGuests?.ToString() ?? "-").Append(':');
			sb.Append(request.MinPrice?.ToString() ?? "-").Append(':');
			sb.Append(request.MaxPrice?.ToString() ?? "-").Append(':');
			sb.Append(request.Page).Append(':').Append(request.PageSize);
			return sb.ToString();
		}
	}
}
