using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Application.Exceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string message) : base(message) { }

		public NotFoundException(string entityName, Guid id)
			: base($"{entityName} with id '{id}' was not found.") { }
	}
}
