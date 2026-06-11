using BookingApi.Application.Models;

namespace BookingApi.Application.Interfaces
{
	public interface IBackgroundJobQueue
	{
		ValueTask EnqueueAsync(NotificationJob job, CancellationToken cancellationToken = default);
		ValueTask<NotificationJob> DequeueAsync(CancellationToken cancellationToken);
	}
}
