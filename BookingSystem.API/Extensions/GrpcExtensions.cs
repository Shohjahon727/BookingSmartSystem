namespace BookingSystem.API.Extensions
{
	public static class GrpcExtensions
	{
		public static IServiceCollection AddGrpcServices(this IServiceCollection services)
		{
			services.AddGrpc();
			return services;
		}
	}
}
