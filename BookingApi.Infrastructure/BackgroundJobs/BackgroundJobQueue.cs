using BookingApi.Application.Interfaces;
using BookingApi.Application.Models;
using System.Threading.Channels;

namespace BookingApi.Infrastructure.BackgroundJobs
{
	public class BackgroundJobQueue : IBackgroundJobQueue
	{
		private readonly Channel<NotificationJob> _queue = Channel.CreateUnbounded<NotificationJob>(
			new UnboundedChannelOptions { SingleReader = true });

		public async ValueTask EnqueueAsync(NotificationJob job, CancellationToken cancellationToken = default)
		{
			await _queue.Writer.WriteAsync(job, cancellationToken);
		}

		public async ValueTask<NotificationJob> DequeueAsync(CancellationToken cancellationToken)
		{
			return await _queue.Reader.ReadAsync(cancellationToken);
		}
	}
}
