using BookingApi.Application.DTOs.Auth;
using BookingApi.Application.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Validators
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Telefon raqam kiritilishi shart")
				.Must(PhoneNumberValidator.IsValid)
				.WithMessage("Telefon raqam noto'g'ri format. Masalan: +998901234567");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Parol kiritilishi shart")
				.MinimumLength(8).WithMessage("Parol kamida 8 ta belgidan iborat bo'lishi kerak")
				.Matches(@"[A-Z]").WithMessage("Parolda kamida bitta katta harf bo'lishi kerak")
				.Matches(@"[a-z]").WithMessage("Parolda kamida bitta kichik harf bo'lishi kerak")
				.Matches(@"[0-9]").WithMessage("Parolda kamida bitta raqam bo'lishi kerak");

			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Ism kiritilishi shart")
				.MinimumLength(3).WithMessage("Ism kamida 3 ta belgidan iborat bo'lishi kerak")
				.MaximumLength(100).WithMessage("Ism 100 ta belgidan oshmasligi kerak");
		}
	}
}
