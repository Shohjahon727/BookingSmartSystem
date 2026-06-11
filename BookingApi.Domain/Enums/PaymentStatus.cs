using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApi.Domain.Enums
{
	public enum PaymentStatus
	{
		Unpaid = 1,       // 💸 To'lanmagan
		Paid = 2,         // ✅ To'landi
		Failed = 3,       // ❌ Xatolik
		Refunded = 4      // 🔄 Qaytarildi
	}
}
