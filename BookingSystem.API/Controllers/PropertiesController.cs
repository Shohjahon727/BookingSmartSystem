using BookingApi.Application.DTOs.Property;
using BookingApi.Application.Interfaces;
using BookingSystem.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PropertiesController : ControllerBase
	{
		private readonly IPropertyService _propertyService;

		public PropertiesController(IPropertyService propertyService)
		{
			_propertyService = propertyService;
		}

		/// <summary>
		/// Barcha uylarni qidirish (Public)
		/// </summary>
		[HttpGet]
		public async Task<ActionResult> Search([FromQuery] SearchPropertyRequest request)
		{
			var (items, totalCount) = await _propertyService.SearchAsync(request);
			return Ok(new { Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize });
		}

		/// <summary>
		/// Uy detali (Public)
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<PropertyResponse>> GetById(Guid id)
		{
			var property = await _propertyService.GetByIdAsync(id);
			if (property == null) return NotFound();
			return Ok(property);
		}

		/// <summary>
		/// Uy qo'shish (Host only)
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Host,Admin")]
		public async Task<ActionResult<PropertyResponse>> Create([FromBody] CreatePropertyRequest request)
		{
			var ownerId = User.GetUserId();
			var property = await _propertyService.CreateAsync(request, ownerId);
			return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
		}

		/// <summary>
		/// Mening uylarim (Host)
		/// </summary>
		[HttpGet("my")]
		[Authorize(Roles = "Host,Admin")]
		public async Task<ActionResult<IEnumerable<PropertyResponse>>> GetMyProperties()
		{
			var ownerId = User.GetUserId();
			var properties = await _propertyService.GetMyPropertiesAsync(ownerId);
			return Ok(properties);
		}

		/// <summary>
		/// Uy yangilash (Host)
		/// </summary>
		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Host,Admin")]
		public async Task<ActionResult<PropertyResponse>> Update(Guid id, [FromBody] CreatePropertyRequest request)
		{
			var ownerId = User.GetUserId();
			var property = await _propertyService.UpdateAsync(id, request, ownerId);
			return Ok(property);
		}

		/// <summary>
		/// Uy o'chirish (Host)
		/// </summary>
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
