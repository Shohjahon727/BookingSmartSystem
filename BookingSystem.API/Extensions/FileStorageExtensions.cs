using BookingApi.Application.Interfaces;
using BookingApi.Infrastructure.Notifications;
using BookingApi.Infrastructure.Storage;

namespace BookingSystem.API.Extensions
{
	public static class FileStorageExtensions
	{
		public static IServiceCollection AddFileStorageServices(this IServiceCollection services, IConfiguration configuration)
		{
			var provider = configuration["FileStorage:Provider"] ?? "Local";

			if (provider.Equals("S3", StringComparison.OrdinalIgnoreCase))
				services.AddScoped<IFileStorageService, S3FileStorageService>();
			else
				services.AddScoped<IFileStorageService, LocalFileStorageService>();

			services.AddScoped<IEmailService, SmtpMockEmailService>();
			return services;
		}

		public static WebApplication UsePropertyImageStaticFiles(this WebApplication app, IConfiguration configuration)
		{
			var provider = configuration["FileStorage:Provider"] ?? "Local";
			if (!provider.Equals("Local", StringComparison.OrdinalIgnoreCase))
				return app;

			var uploadPath = configuration["FileStorage:LocalPath"]
				?? Path.Combine(app.Environment.ContentRootPath, "uploads", "properties");

			Directory.CreateDirectory(uploadPath);

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadPath),
				RequestPath = "/uploads/properties"
			});

			return app;
		}
	}
}
