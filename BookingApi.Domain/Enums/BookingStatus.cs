using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Enums
{
	public enum BookingStatus
	{
		Pending = 1,      // ⏳ Kutilmoqda (to'lov kutilmoqda)
		Confirmed = 2,    // ✅ Tasdiqlandi
		Cancelled = 3,    // ❌ Bekor qilindi
		Completed = 4     // 🏁 Tugallandi (mehmon ketdi)
	}
}
