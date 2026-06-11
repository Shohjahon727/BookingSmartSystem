using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using BookingApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookingApi.Infrastructure.Storage
{
	public class S3FileStorageService : IFileStorageService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly string _bucketName;
		private readonly string _publicBaseUrl;
		private readonly string _keyPrefix;
		private readonly ILogger<S3FileStorageService> _logger;

		public S3FileStorageService(IConfiguration configuration, ILogger<S3FileStorageService> logger)
		{
			_logger = logger;
			_bucketName = configuration["AwsS3:BucketName"] ?? throw new InvalidOperationException("AwsS3:BucketName is required");
			_keyPrefix = configuration["AwsS3:KeyPrefix"] ?? "properties/";
			_publicBaseUrl = configuration["AwsS3:PublicBaseUrl"]
				?? $"https://{_bucketName}.s3.amazonaws.com/{_keyPrefix.TrimEnd('/')}";

			var region = configuration["AwsS3:Region"] ?? "us-east-1";
			var accessKey = configuration["AwsS3:AccessKey"];
			var secretKey = configuration["AwsS3:SecretKey"];

			_s3Client = !string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey)
				? new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region))
				: new AmazonS3Client(RegionEndpoint.GetBySystemName(region));
		}

		public async Task<string> SavePropertyImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			var key = $"{_keyPrefix.TrimEnd('/')}/{Guid.NewGuid():N}{extension}";

			var request = new PutObjectRequest
			{
				BucketName = _bucketName,
				Key = key,
				InputStream = fileStream,
				ContentType = contentType,
				CannedACL = S3CannedACL.PublicRead
			};

			await _s3Client.PutObjectAsync(request, cancellationToken);

			var url = $"{_publicBaseUrl.TrimEnd('/')}/{Path.GetFileName(key)}";
			_logger.LogInformation("S3 image uploaded: {Key} -> {Url}", key, url);
			return url;
		}

		public async Task DeletePropertyImageAsync(string imageUrl, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(imageUrl))
				return;

			var fileName = Path.GetFileName(imageUrl);
			if (string.IsNullOrWhiteSpace(fileName))
				return;

			var key = $"{_keyPrefix.TrimEnd('/')}/{fileName}";

			try
			{
				await _s3Client.DeleteObjectAsync(_bucketName, key, cancellationToken);
				_logger.LogInformation("S3 image deleted: {Key}", key);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "S3 delete failed for key {Key}", key);
			}
		}
	}
}
