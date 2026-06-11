using BookingApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.Storage
{
	public class LocalFileStorageService : IFileStorageService
	{
		private readonly string _uploadPath;
		private readonly string _publicBaseUrl;
		private readonly ILogger<LocalFileStorageService> _logger;

		public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
		{
			_uploadPath = configuration["FileStorage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "properties");
			_publicBaseUrl = configuration["FileStorage:PublicBaseUrl"] ?? "/uploads/properties";
			_logger = logger;

			Directory.CreateDirectory(_uploadPath);
		}

		public async Task<string> SavePropertyImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			var uniqueName = $"{Guid.NewGuid():N}{extension}";
			var fullPath = Path.Combine(_uploadPath, uniqueName);

			await using var output = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
			await fileStream.CopyToAsync(output, cancellationToken);

			var url = $"{_publicBaseUrl.TrimEnd('/')}/{uniqueName}";
			_logger.LogInformation("Property image saved: {Url}", url);
			return url;
		}

		public Task DeletePropertyImageAsync(string imageUrl, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(imageUrl))
				return Task.CompletedTask;

			var fileName = Path.GetFileName(imageUrl);
			if (string.IsNullOrWhiteSpace(fileName))
				return Task.CompletedTask;

			var fullPath = Path.Combine(_uploadPath, fileName);
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
				_logger.LogInformation("Property image deleted: {Path}", fullPath);
			}

			return Task.CompletedTask;
		}
	}
}
