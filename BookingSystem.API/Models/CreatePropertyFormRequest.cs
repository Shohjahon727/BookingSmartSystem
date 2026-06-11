using BookingApi.Application.DTOs.Property;

namespace BookingSystem.API.Models
{
	public class CreatePropertyFormRequest
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public decimal PricePerNight { get; set; }
		public int MaxGuests { get; set; }
		public int Bedrooms { get; set; }
		public int Bathrooms { get; set; }
		public IFormFile? Image { get; set; }

		public CreatePropertyRequest ToDto() => new()
		{
			Title = Title,
			Description = Description,
			Address = Address,
			City = City,
			Country = Country,
			PricePerNight = PricePerNight,
			MaxGuests = MaxGuests,
			Bedrooms = Bedrooms,
			Bathrooms = Bathrooms
		};
	}
}
