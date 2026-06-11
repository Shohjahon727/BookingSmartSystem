using BookingApi.Domain.Entities;

namespace BookingApi.Application.Interfaces
{
	public interface IPropertySearchService
	{
		bool IsEnabled { get; }
		Task<(IEnumerable<Property> Items, int TotalCount)> SearchAsync(
			string? city,
			DateTime? availableFrom,
			DateTime? availableTo,
			int? maxGuests,
			decimal? minPrice,
			decimal? maxPrice,
			int page = 1,
			int pageSize = 10,
			CancellationToken cancellationToken = default);
		Task IndexPropertyAsync(Property property, CancellationToken cancellationToken = default);
		Task DeleteFromIndexAsync(Guid propertyId, CancellationToken cancellationToken = default);
		Task ReindexAllAsync(CancellationToken cancellationToken = default);
	}
}
