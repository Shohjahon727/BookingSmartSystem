using BookingApi.Application.Interfaces;
using Grpc.Core;

namespace BookingSystem.API.Grpc
{
	public class PropertyGrpcService : PropertyGrpc.PropertyGrpcBase
	{
		private readonly IPropertyService _propertyService;

		public PropertyGrpcService(IPropertyService propertyService)
		{
			_propertyService = propertyService;
		}

		public override async Task<PropertyReply> GetProperty(GetPropertyRequest request, ServerCallContext context)
		{
			if (!Guid.TryParse(request.Id, out var id))
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid property id"));

			var property = await _propertyService.GetByIdAsync(id);
			if (property == null)
				throw new RpcException(new Status(StatusCode.NotFound, "Property not found"));

			return MapToReply(property);
		}

		public override async Task<SearchPropertiesReply> SearchProperties(SearchPropertiesRequest request, ServerCallContext context)
		{
			var (items, totalCount) = await _propertyService.SearchAsync(new BookingApi.Application.DTOs.Property.SearchPropertyRequest
			{
				City = string.IsNullOrWhiteSpace(request.City) ? null : request.City,
				MinPrice = request.MinPrice > 0 ? (decimal)request.MinPrice : null,
				MaxPrice = request.MaxPrice > 0 ? (decimal)request.MaxPrice : null,
				MaxGuests = request.MaxGuests > 0 ? request.MaxGuests : null,
				Page = request.Page > 0 ? request.Page : 1,
				PageSize = request.PageSize > 0 ? request.PageSize : 10
			});

			var reply = new SearchPropertiesReply { TotalCount = totalCount };
			reply.Items.AddRange(items.Select(MapToReply));
			return reply;
		}

		private static PropertyReply MapToReply(BookingApi.Application.DTOs.Property.PropertyResponse p) => new()
		{
			Id = p.Id.ToString(),
			Title = p.Title,
			Description = p.Description,
			City = p.City,
			Country = p.Country,
			PricePerNight = (double)p.PricePerNight,
			MaxGuests = p.MaxGuests,
			Bedrooms = p.Bedrooms,
			Bathrooms = p.Bathrooms,
			ImageUrl = p.ImageUrl ?? string.Empty
		};
	}
}
