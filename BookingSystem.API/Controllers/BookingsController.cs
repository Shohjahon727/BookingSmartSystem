using BookingApi.Application.DTOs.Booking;
using BookingApi.Application.Interfaces;
using BookingSystem.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]  // 🔒 Barcha endpointlar autentifikatsiya talab qiladi
	public class BookingsController : ControllerBase
	{
		private readonly IBookingService _bookingService;

		public BookingsController(IBookingService bookingService)
		{
			_bookingService = bookingService;
		}

		/// <summary>
		/// Bron qilish (Guest)
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest request)
		{
			var guestId = User.GetUserId();
			var booking = await _bookingService.CreateAsync(request, guestId);
			return Ok(booking);
		}

		/// <summary>
		/// Bron detali
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<BookingResponse>> GetById(Guid id)
		{
			var guestId = User.GetUserId();
			var booking = await _bookingService.GetByIdAsync(id, guestId);
			if (booking == null) return NotFound();
			return Ok(booking);
		}

		/// <summary>
		/// Mening bronlarim
		/// </summary>
		[HttpGet("my")]
		public async Task<ActionResult<IEnumerable<BookingResponse>>> GetMyBookings()
		{
			var guestId = User.GetUserId();
			var bookings = await _bookingService.GetMyBookingsAsync(guestId);
			return Ok(bookings);
		}

		/// <summary>
		/// Bron bekor qilish
		/// </summary>
		[HttpPost("{id:guid}/cancel")]
		public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelBookingRequest? request)
		{
			var guestId = User.GetUserId();
			await _bookingService.CancelAsync(id, guestId, request?.Reason);
			return NoContent();
		}
	}
}
