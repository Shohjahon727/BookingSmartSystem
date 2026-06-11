using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookingApi.Application.Helpers
{
	public static class PhoneNumberValidator
	{
		private static readonly Regex PhoneRegex = new Regex(
		@"^(\+998|998)?[0-9]{9}$",
		RegexOptions.Compiled);

		public static bool IsValid(string phoneNumber)
		{
			if (string.IsNullOrWhiteSpace(phoneNumber))
				return false;

			var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
			return digitsOnly.Length == 12 || digitsOnly.Length == 9;
		}

		public static string Normalize(string phoneNumber)
		{
			var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

			if (digitsOnly.Length == 9)
				return "+998" + digitsOnly;

			if (digitsOnly.Length == 12 && digitsOnly.StartsWith("998"))
				return "+" + digitsOnly;

			return phoneNumber.StartsWith('+') ? phoneNumber : "+" + digitsOnly;
		}

	}
}
