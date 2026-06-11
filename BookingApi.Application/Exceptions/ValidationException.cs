using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Exceptions
{
	public class ValidationException : Exception
	{
		public Dictionary<string, string[]> Errors { get; }

		public ValidationException(Dictionary<string, string[]> errors)
			: base("Validation failed")
		{
			Errors = errors;
		}
	}
}
