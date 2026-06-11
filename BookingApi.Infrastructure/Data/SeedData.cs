using BookingApi.Domain.Entities;
using BookingApi.Domain.Enums;
using BookingApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Data
{
	public static class SeedData
	{
		public static async Task SeedAsync(BookingDbContext context)
		{
			var host = await EnsureUserAsync(context, "+998902222222", "Host1234", "Azizbek Rahimov", UserRole.Host);
			await EnsureUserAsync(context, "+998901111111", "Admin1234", "Admin User", UserRole.Admin);
			await EnsureUserAsync(context, "+998903333333", "Guest1234", "Dilshod Karimov", UserRole.Guest);

			if (await context.Properties.AnyAsync())
				return;

			var properties = new List<Property>
			{
				new Property
				{
					Title = "Luxury Apartment in Tashkent",
					Description = "Zamonaviy va qulay kvartira. WiFi, konditsioner, TV.",
					Address = "Amir Temur ko'chasi 45",
					City = "Tashkent",
					Country = "Uzbekistan",
					PricePerNight = 80,
					MaxGuests = 4,
					MinGuests = 1,
					Bedrooms = 2,
					Bathrooms = 1,
					ImageUrl = "https://example.com/apt1.jpg",
					OwnerId = host.Id
				},
				new Property
				{
					Title = "Cozy Villa in Samarkand",
					Description = "Tarixiy shaharda xususiy villa. Hovli, basseyn.",
					Address = "Registon ko'chasi 12",
					City = "Samarkand",
					Country = "Uzbekistan",
					PricePerNight = 120,
					MaxGuests = 6,
					MinGuests = 1,
					Bedrooms = 3,
					Bathrooms = 2,
					ImageUrl = "https://example.com/villa1.jpg",
					OwnerId = host.Id
				},
				new Property
				{
					Title = "Modern Studio in Bukhara",
					Description = "Markazda yangi studio kvartira. Hammasi yaqin.",
					Address = "Lyabi House 8",
					City = "Bukhara",
					Country = "Uzbekistan",
					PricePerNight = 45,
					MaxGuests = 2,
					MinGuests = 1,
					Bedrooms = 1,
					Bathrooms = 1,
					ImageUrl = "https://example.com/studio1.jpg",
					OwnerId = host.Id
				}
			};

			context.Properties.AddRange(properties);
			await context.SaveChangesAsync();
		}

		private static async Task<User> EnsureUserAsync(
			BookingDbContext context,
			string phoneNumber,
			string password,
			string fullName,
			UserRole role)
		{
			var existing = await context.Users
				.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);

			if (existing != null)
				return existing;

			var user = new User
			{
				PhoneNumber = phoneNumber,
				PasswordHash = PasswordHasher.HashPassword(password),
				FullName = fullName,
				Role = role
			};

			context.Users.Add(user);
			await context.SaveChangesAsync();
			return user;
		}
	}
}
