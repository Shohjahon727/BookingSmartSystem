using BookingApi.Application.DTOs.Property;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Validators
{
	public class CreatePropertyRequestValidator : AbstractValidator<CreatePropertyRequest>
	{
		public CreatePropertyRequestValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Sarlavha kiritilishi shart")
				.MinimumLength(5).WithMessage("Sarlavha kamida 5 ta belgidan iborat bo'lishi kerak")
				.MaximumLength(200).WithMessage("Sarlavha 200 ta belgidan oshmasligi kerak");

			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Tavsif kiritilishi shart")
				.MinimumLength(20).WithMessage("Tavsif kamida 20 ta belgidan iborat bo'lishi kerak");

			RuleFor(x => x.City)
				.NotEmpty().WithMessage("Shahar kiritilishi shart");

			RuleFor(x => x.Country)
				.NotEmpty().WithMessage("Davlat kiritilishi shart");

			RuleFor(x => x.PricePerNight)
				.GreaterThan(0).WithMessage("Narx 0 dan katta bo'lishi kerak")
				.LessThanOrEqualTo(100000).WithMessage("Narx juda katta");

			RuleFor(x => x.MaxGuests)
				.GreaterThan(0).WithMessage("Mehmonlar soni 0 dan katta bo'lishi kerak")
				.LessThanOrEqualTo(50).WithMessage("Mehmonlar soni 50 dan oshmasligi kerak");

			RuleFor(x => x.Bedrooms)
				.GreaterThanOrEqualTo(0).WithMessage("Yotoqxonalar soni manfiy bo'lishi mumkin emas");

			RuleFor(x => x.Bathrooms)
				.GreaterThanOrEqualTo(0).WithMessage("Hammomlar soni manfiy bo'lishi mumkin emas");
		}
	}
}
