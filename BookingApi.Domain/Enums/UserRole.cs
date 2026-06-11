using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Enums
{
	public enum UserRole
	{
		Guest = 1,   // 🏠 Mehmon (bron qiluvchi)
		Host = 2,    // 🏨 Uy egasi
		Admin = 3    // 👑 Admin
	}
}
