using BookingApi.Application.DTOs.Booking;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Validators
{
	public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
	{
		public CreateBookingRequestValidator()
		{
			RuleFor(x => x.PropertyId)
				.NotEmpty().WithMessage("Uy tanlanishi shart");

			RuleFor(x => x.CheckInDate)
				.NotEmpty().WithMessage("Kirish sanasi kiritilishi shart")
				.GreaterThan(DateTime.UtcNow.Date).WithMessage("Kirish sanasi bugundan keyin bo'lishi kerak");

			RuleFor(x => x.CheckOutDate)
				.NotEmpty().WithMessage("Chiqish sanasi kiritilishi shart")
				.GreaterThan(x => x.CheckInDate).WithMessage("Chiqish sanasi kirish sanasidan keyin bo'lishi kerak");

			RuleFor(x => x.GuestCount)
				.GreaterThan(0).WithMessage("Mehmonlar soni 0 dan katta bo'lishi kerak");
		}
	}
}
