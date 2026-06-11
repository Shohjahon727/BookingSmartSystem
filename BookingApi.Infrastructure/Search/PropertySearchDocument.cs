namespace BookingApi.Infrastructure.Search
{
	public class PropertySearchDocument
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public decimal PricePerNight { get; set; }
		public int MaxGuests { get; set; }
		public int Bedrooms { get; set; }
		public int Bathrooms { get; set; }
		public string? ImageUrl { get; set; }
		public Guid OwnerId { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsDeleted { get; set; }
	}
}
