using BookingApi.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace BookingSystem.API.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unhandled exception occurred");
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

			var statusCode = exception switch
			{
				NotFoundException => HttpStatusCode.NotFound,           // 404
				BadRequestException => HttpStatusCode.BadRequest,      // 400
				ValidationException => HttpStatusCode.BadRequest,      // 400
				UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
				_ => HttpStatusCode.InternalServerError               // 500
			};

			context.Response.StatusCode = (int)statusCode;

			object response = exception is ValidationException validationException
				? new
				{
					StatusCode = context.Response.StatusCode,
					Message = validationException.Message,
					Errors = validationException.Errors,
					StackTrace = context.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true
						? validationException.StackTrace
						: null
				}
				: new
				{
					StatusCode = context.Response.StatusCode,
					Message = exception.Message,
					StackTrace = context.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true
						? exception.StackTrace
						: null
				};

			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
		}
	}
}
