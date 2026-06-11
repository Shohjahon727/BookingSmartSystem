namespace BookingApi.Application.Interfaces
{
	public interface IFileStorageService
	{
		Task<string> SavePropertyImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
		Task DeletePropertyImageAsync(string imageUrl, CancellationToken cancellationToken = default);
	}
}
