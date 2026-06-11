using BookingApi.Application.Interfaces;
using Grpc.Core;

namespace BookingSystem.API.Grpc
{
	public class BookingGrpcService : BookingGrpc.BookingGrpcBase
	{
		private readonly IBookingRepository _bookingRepository;

		public BookingGrpcService(IBookingRepository bookingRepository)
		{
			_bookingRepository = bookingRepository;
		}

		public override async Task<CheckAvailabilityReply> CheckAvailability(CheckAvailabilityRequest request, ServerCallContext context)
		{
			if (!Guid.TryParse(request.PropertyId, out var propertyId))
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid property id"));

			if (!DateTime.TryParse(request.CheckIn, out var checkIn) || !DateTime.TryParse(request.CheckOut, out var checkOut))
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid dates"));

			var isAvailable = await _bookingRepository.IsPropertyAvailableAsync(propertyId, checkIn, checkOut);

			return new CheckAvailabilityReply
			{
				IsAvailable = isAvailable,
				Message = isAvailable ? "Uy mavjud" : "Tanlangan sanalarda band"
			};
		}

		public override Task<BookingReply> GetBooking(GetBookingRequest request, ServerCallContext context)
		{
			// gRPC orqali booking olish — microservice inter-service communication demo
			throw new RpcException(new Status(StatusCode.Unimplemented, "Use REST API with JWT for booking details"));
		}
	}
}
