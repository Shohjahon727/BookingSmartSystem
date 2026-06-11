using BookingApi.Application.Interfaces;
using BookingApi.Application.Validators;
using BookingApi.Infrastructure.Data;
using BookingApi.Infrastructure.Repositories;
using BookingApi.Infrastructure.Services;
using BookingSystem.API.Extensions;
using BookingSystem.API.Grpc;
using BookingSystem.API.Hubs;
using BookingSystem.API.Swagger;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookingDbContext>(options =>
	options.UseNpgsql(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		b => b.MigrationsAssembly("BookingApi.Infrastructure")
	));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddCachingServices(builder.Configuration);
builder.Services.AddBackgroundJobServices();
builder.Services.AddFileStorageServices(builder.Configuration);
builder.Services.AddElasticsearchServices(builder.Configuration);
builder.Services.AddGrpcServices();
builder.Services.AddPrometheusMetrics();
builder.Services.AddSignalRServices();
builder.Services.AddRateLimitingServices(builder.Configuration);
builder.Services.AddApiVersioningServices();
builder.Services.AddHealthCheckServices(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var key = Encoding.UTF8.GetBytes(jwtSecret);
var signalREvents = SignalRExtensions.CreateSignalRJwtEvents();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ClockSkew = TimeSpan.Zero
	};

	options.Events = signalREvents;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Description = "JWT token ni kiriting: Bearer {token}"
	});

	options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
	{
		{
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
	await context.Database.MigrateAsync();
	await SeedData.SeedAsync(context);
}

app.UseGlobalExceptionHandler();
app.UseStaticFiles();
app.UsePropertyImageStaticFiles(builder.Configuration);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		var provider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();
		foreach (var description in provider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint(
				$"/swagger/{description.GroupName}/swagger.json",
				$"Booking API {description.GroupName.ToUpperInvariant()}");
		}
	});
}

app.UseHttpsRedirection();
app.UsePrometheusMetrics();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthCheckEndpoints();
app.MapControllers();
app.MapHub<BookingNotificationHub>("/hubs/bookings");
app.MapGrpcService<PropertyGrpcService>();
app.MapGrpcService<BookingGrpcService>();

app.Run();
