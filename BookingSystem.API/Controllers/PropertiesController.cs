using Asp.Versioning;
using BookingApi.Application.DTOs.Property;
using BookingApi.Application.Helpers;
using BookingApi.Application.Interfaces;
using BookingSystem.API.Extensions;
using BookingSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	[Route("api/[controller]")]
	public class PropertiesController : ControllerBase
	{
		private readonly IPropertyService _propertyService;
		private readonly IConfiguration _configuration;

		public PropertiesController(IPropertyService propertyService, IConfiguration configuration)
		{
			_propertyService = propertyService;
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<ActionResult> Search([FromQuery] SearchPropertyRequest request)
		{
			var (items, totalCount) = await _propertyService.SearchAsync(request);
			return Ok(new { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize });
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<PropertyResponse>> GetById(Guid id)
		{
			var property = await _propertyService.GetByIdAsync(id);
			if (property == null) return NotFound();
			return Ok(property);
		}

		[HttpPost]
		[Authorize(Roles = "Host,Admin")]
		public async Task<ActionResult<PropertyResponse>> Create([FromBody] CreatePropertyRequest request)
		{
			var ownerId = User.GetUserId();
			var property = await _propertyService.CreateAsync(request, ownerId);
			return CreatedAtAction(nameof(GetById), new { id = property.Id, version = "1.0" }, property);
		}

		/// <summary>
		/// Uy + rasm birga yuklash (multipart/form-data)
		/// </summary>
		[HttpPost("with-image")]
		[Authorize(Roles = "Host,Admin")]
		[Consumes("multipart/form-data")]
		public async Task<ActionResult<PropertyResponse>> CreateWithImage([FromForm] CreatePropertyFormRequest request)
		{
			var ownerId = User.GetUserId();
			var property = await _propertyService.CreateAsync(request.ToDto(), ownerId);

			if (request.Image != null)
			{
				var maxMb = _configuration.GetValue("FileStorage:MaxImageSizeMb", 5);
				ImageFileValidator.Validate(request.Image.FileName, request.Image.ContentType, request.Image.Length, maxMb);

				await using var stream = request.Image.OpenReadStream();
				property = await _propertyService.UploadImageAsync(
					property.Id, stream, request.Image.FileName, request.Image.ContentType, request.Image.Length, ownerId);
			}

			return CreatedAtAction(nameof(GetById), new { id = property.Id, version = "1.0" }, property);
		}

		/// <summary>
		/// Mavjud uyga rasm yuklash (IFormFile)
		/// </summary>
		[HttpPost("{id:guid}/image")]
		[Authorize(Roles = "Host,Admin")]
		[Consumes("multipart/form-data")]
		public async Task<ActionResult<PropertyResponse>> UploadImage(Guid id, IFormFile image)
		{
			if (image == null)
				return BadRequest(new { message = "Rasm fayli yuborilmadi" });

			var maxMb = _configuration.GetValue("FileStorage:MaxImageSizeMb", 5);
			ImageFileValidator.Validate(image.FileName, image.ContentType, image.Length, maxMb);

			var ownerId = User.GetUserId();
			await using var stream = image.OpenReadStream();
			var property = await _propertyService.UploadImageAsync(
				id, stream, image.FileName, image.ContentType, image.Length, ownerId);

			return Ok(property);
		}

		[HttpGet("my")]
		[Authorize(Roles = "Host,Admin")]
		public async Task<ActionResult<IEnumerable<PropertyResponse>>> GetMyProperties()
		{
			var ownerId = User.GetUserId();
			var properties = await _propertyService.GetMyPropertiesAsync(ownerId);
			return Ok(properties);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Host,Admin")]
		public async Task<ActionResult<PropertyResponse>> Update(Guid id, [FromBody] CreatePropertyRequest request)
		{
			var ownerId = User.GetUserId();
			var property = await _propertyService.UpdateAsync(id, request, ownerId);
			return Ok(property);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Host,Admin")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var ownerId = User.GetUserId();
			await _propertyService.DeleteAsync(id, ownerId);
			return NoContent();
		}
	}
}
