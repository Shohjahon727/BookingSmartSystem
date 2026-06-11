using BookingApi.Application.DTOs.Auth;
using BookingApi.Application.Exceptions;
using BookingApi.Application.Helpers;
using BookingApi.Application.Interfaces;
using BookingApi.Application.Models;
using BookingApi.Domain.Entities;
using BookingApi.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace BookingApi.Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly JwtService _jwtService;
		private readonly IBackgroundJobQueue _backgroundJobQueue;
		private readonly int _expiryMinutes;

		public AuthService(
			IUserRepository userRepository,
			IUnitOfWork unitOfWork,
			JwtService jwtService,
			IBackgroundJobQueue backgroundJobQueue,
			IConfiguration configuration)
		{
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
			_jwtService = jwtService;
			_backgroundJobQueue = backgroundJobQueue;
			_expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60");
		}

		public async Task<AuthResponse> LoginAsync(LoginRequest request)
		{
			var normalizedPhone = PhoneNumberValidator.Normalize(request.PhoneNumber);

			var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone);

			if (user == null)
				throw new BadRequestException("Telefon raqam yoki parol noto'g'ri");

			if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
				throw new BadRequestException("Telefon raqam yoki parol noto'g'ri");

			return BuildAuthResponse(user);
		}

		public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
		{
			var normalizedPhone = PhoneNumberValidator.Normalize(request.PhoneNumber);

			if (await _userRepository.PhoneNumberExistsAsync(normalizedPhone))
				throw new BadRequestException("Bu telefon raqam allaqachon ro'yxatdan o'tgan");

			var user = new User
			{
				PhoneNumber = normalizedPhone,
				PasswordHash = PasswordHasher.HashPassword(request.Password),
				FullName = request.FullName,
				Role = UserRole.Guest
			};

			await _userRepository.AddAsync(user);
			await _unitOfWork.SaveChangesAsync();

			await _backgroundJobQueue.EnqueueAsync(new NotificationJob
			{
				Channel = NotificationChannel.Email,
				Recipient = user.PhoneNumber,
				Subject = "Xush kelibsiz!",
				Message = $"Salom {user.FullName}! Booking Smart System ga muvaffaqiyatli ro'yxatdan o'tdingiz."
			});

			return BuildAuthResponse(user);
		}

		private AuthResponse BuildAuthResponse(User user)
		{
			return new AuthResponse
			{
				Token = _jwtService.GenerateToken(user),
				RefreshToken = _jwtService.GenerateRefreshToken(),
				ExpiresAt = DateTime.UtcNow.AddMinutes(_expiryMinutes),
				PhoneNumber = user.PhoneNumber,
				FullName = user.FullName,
				Role = user.Role.ToString()
			};
		}
	}
}
