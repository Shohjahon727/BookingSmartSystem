using BookingApi.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Validators
{
	public class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{
			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Telefon raqam kiritilishi shart");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Parol kiritilishi shart");
		}
	}
}
