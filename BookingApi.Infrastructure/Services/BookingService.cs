using BookingApi.Application.DTOs.Booking;
using BookingApi.Application.Exceptions;
using BookingApi.Application.Interfaces;
using BookingApi.Application.Models;
using BookingApi.Domain.Entities;
using BookingApi.Domain.Enums;

namespace BookingApi.Infrastructure.Services
{
	public class BookingService : IBookingService
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IPropertyRepository _propertyRepository;
		private readonly IUserRepository _userRepository;
		private readonly IPaymentService _paymentService;
		private readonly IBackgroundJobQueue _backgroundJobQueue;
		private readonly IUnitOfWork _unitOfWork;

		public BookingService(
			IBookingRepository bookingRepository,
			IPropertyRepository propertyRepository,
			IUserRepository userRepository,
			IPaymentService paymentService,
			IBackgroundJobQueue backgroundJobQueue,
			IUnitOfWork unitOfWork)
		{
			_bookingRepository = bookingRepository;
			_propertyRepository = propertyRepository;
			_userRepository = userRepository;
			_paymentService = paymentService;
			_backgroundJobQueue = backgroundJobQueue;
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

			await EnqueueBookingNotificationAsync(booking, property.Title, paymentResult.Success);

			booking.Property = property;
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

			var guest = await _userRepository.GetByIdAsync(guestId);
			if (guest != null)
			{
				await _backgroundJobQueue.EnqueueAsync(new NotificationJob
				{
					Channel = NotificationChannel.Sms,
					Recipient = guest.PhoneNumber,
					Subject = "Bron bekor qilindi",
					Message = $"Bron #{id.ToString()[..8]} bekor qilindi. Sabab: {booking.CancellationReason}"
				});
			}
		}

		private async Task EnqueueBookingNotificationAsync(Booking booking, string propertyTitle, bool paymentSuccess)
		{
			var guest = await _userRepository.GetByIdAsync(booking.GuestId);
			if (guest == null)
				return;

			if (paymentSuccess)
			{
				await _backgroundJobQueue.EnqueueAsync(new NotificationJob
				{
					Channel = NotificationChannel.Email,
					Recipient = guest.PhoneNumber,
					Subject = "Bron tasdiqlandi",
					Message = $"{propertyTitle} uchun broningiz tasdiqlandi. Summa: {booking.TotalPrice:N0} so'm."
				});

				await _backgroundJobQueue.EnqueueAsync(new NotificationJob
				{
					Channel = NotificationChannel.Sms,
					Recipient = guest.PhoneNumber,
					Subject = "Bron tasdiqlandi",
					Message = $"Bron tasdiqlandi: {propertyTitle}, {booking.CheckInDate:dd.MM} - {booking.CheckOutDate:dd.MM}"
				});
			}
			else
			{
				await _backgroundJobQueue.EnqueueAsync(new NotificationJob
				{
					Channel = NotificationChannel.Sms,
					Recipient = guest.PhoneNumber,
					Subject = "To'lov xatolik",
					Message = $"Bron uchun to'lov amalga oshmadi. Qayta urinib ko'ring."
				});
			}
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
