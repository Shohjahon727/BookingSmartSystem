using BookingApi.Application.Exceptions;

namespace BookingApi.Application.Helpers
{
	public static class ImageFileValidator
	{
		private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
		{
			".jpg", ".jpeg", ".png", ".webp"
		};

		private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
		{
			"image/jpeg", "image/png", "image/webp"
		};

		public static void Validate(string fileName, string contentType, long fileSizeBytes, long maxSizeMb = 5)
		{
			if (string.IsNullOrWhiteSpace(fileName))
				throw new BadRequestException("Fayl nomi bo'sh bo'lishi mumkin emas");

			var extension = Path.GetExtension(fileName);
			if (!AllowedExtensions.Contains(extension))
				throw new BadRequestException("Faqat JPG, PNG yoki WEBP formatlari qabul qilinadi");

			if (!AllowedContentTypes.Contains(contentType))
				throw new BadRequestException("Noto'g'ri fayl turi. Faqat rasm yuklang");

			var maxBytes = maxSizeMb * 1024 * 1024;
			if (fileSizeBytes <= 0)
				throw new BadRequestException("Fayl bo'sh");

			if (fileSizeBytes > maxBytes)
				throw new BadRequestException($"Rasm hajmi {maxSizeMb}MB dan oshmasligi kerak");
		}
	}
}
