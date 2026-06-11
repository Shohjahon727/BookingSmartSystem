using BookingApi.Application.DTOs.Booking;
using BookingApi.Application.Exceptions;
using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Infrastructure.Services
{
	public class BookingService : IBookingService
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IPropertyRepository _propertyRepository;
		private readonly IPaymentService _paymentService;
		private readonly IUnitOfWork _unitOfWork;

		public BookingService(
			IBookingRepository bookingRepository,
			IPropertyRepository propertyRepository,
			IPaymentService paymentService,
			IUnitOfWork unitOfWork)
		{
			_bookingRepository = bookingRepository;
			_propertyRepository = propertyRepository;
			_paymentService = paymentService;
			_unitOfWork = unitOfWork;
		}

		public async Task<BookingResponse> CreateAsync(CreateBookingRequest request, Guid guestId)
		{
			// 1️⃣ Property mavjudmi?
			var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
			if (property == null)
				throw new NotFoundException("Property", request.PropertyId);

			// 2️⃣ Sana validatsiyasi
			if (request.CheckInDate >= request.CheckOutDate)
				throw new BadRequestException("Chiqish sanasi kirish sanasidan keyin bo'lishi kerak");

			if (request.CheckInDate.Date <= DateTime.UtcNow.Date)
				throw new BadRequestException("Kirish sanasi bugundan keyin bo'lishi kerak");

			// 3️⃣ Mehmonlar soni
			if (request.GuestCount > property.MaxGuests)
				throw new BadRequestException($"Bu uyda maksimum {property.MaxGuests} mehmon qabul qilinadi");

			// 4️⃣ 🔥🔥🔥 DATE OVERLAP CHECK
			var isAvailable = await _bookingRepository.IsPropertyAvailableAsync(
				request.PropertyId,
				request.CheckInDate,
				request.CheckOutDate);

			if (!isAvailable)
				throw new BadRequestException("Tanlangan sanalarda bu uy band. Boshqa sanani tanlang.");

			// 5️⃣ Hisob-kitob
			var numberOfNights = (request.CheckOutDate - request.CheckInDate).Days;
			var totalPrice = numberOfNights * property.PricePerNight;

			// 6️⃣ Booking yaratish
			var booking = new Booking
			{
				PropertyId = request.PropertyId,
				GuestId = guestId,
				CheckInDate = request.CheckInDate,
				CheckOutDate = request.CheckOutDate,
				NumberOfNights = numberOfNights,
				TotalPrice = totalPrice,
				GuestCount = request.GuestCount,
				Status = BookingStatus.Pending,
				PaymentStatus = PaymentStatus.Unpaid
			};

			await _bookingRepository.AddAsync(booking);
			await _unitOfWork.SaveChangesAsync();

			// 7️⃣ 💰 Auto payment (Mock)
			var paymentResult = await _paymentService.ProcessPaymentAsync(booking.Id, totalPrice);

			if (paymentResult.Success)
			{
				// ✅ To'lov muvaffaqiyatli
				booking.Status = BookingStatus.Confirmed;
				booking.PaymentStatus = PaymentStatus.Paid;
			}
			else
			{
				booking.Status = BookingStatus.Cancelled;
				booking.PaymentStatus = PaymentStatus.Failed;
				booking.CancellationReason = $"To'lov xatolik: {paymentResult.Message}";
			}

			_bookingRepository.UpdateAsync(booking);
			await _unitOfWork.SaveChangesAsync();

			return MapToResponse(booking);
		}

		public async Task<BookingResponse?> GetByIdAsync(Guid id, Guid guestId)
		{
			var booking = await _bookingRepository.GetByIdWithDetailsAsync(id);
			if (booking == null)
				return null;

			if (booking.GuestId != guestId)
				throw new UnauthorizedAccessException("Bu bron sizga tegishli emas");

			return MapToResponse(booking);
		}

		public async Task<IEnumerable<BookingResponse>> GetMyBookingsAsync(Guid guestId)
		{
			var bookings = await _bookingRepository.GetUserBookingsAsync(guestId);
			return bookings.Select(MapToResponse);
		}

		public async Task CancelAsync(Guid id, Guid guestId, string? reason = null)
		{
			var booking = await _bookingRepository.GetByIdAsync(id);
			if (booking == null)
				throw new NotFoundException("Booking", id);

			if (booking.GuestId != guestId)
				throw new UnauthorizedAccessException("Bu bron sizga tegishli emas");

			if (booking.Status == BookingStatus.Cancelled)
				throw new BadRequestException("Bu bron allaqachon bekor qilingan");

			if (booking.Status == BookingStatus.Completed)
				throw new BadRequestException("Tugallangan bronni bekor qilib bo'lmaydi");

			booking.Status = BookingStatus.Cancelled;
			booking.CancellationReason = reason ?? "Foydalanuvchi tomonidan bekor qilindi";
			booking.PaymentStatus = PaymentStatus.Refunded;

			_bookingRepository.UpdateAsync(booking);
			await _unitOfWork.SaveChangesAsync();
		}

		private static BookingResponse MapToResponse(Booking booking)
		{
			return new BookingResponse
			{
				Id = booking.Id,
				PropertyId = booking.PropertyId,
				PropertyTitle = booking.Property?.Title ?? "Noma'lum",
				CheckInDate = booking.CheckInDate,
				CheckOutDate = booking.CheckOutDate,
				NumberOfNights = booking.NumberOfNights,
				TotalPrice = booking.TotalPrice,
				GuestCount = booking.GuestCount,
				Status = booking.Status,
				PaymentStatus = booking.PaymentStatus,
				CreatedAt = booking.CreatedAt
			};
		}
	}
}
