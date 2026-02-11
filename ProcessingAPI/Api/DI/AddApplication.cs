using ProcessingAPI.Services;

namespace ProcessingAPI.DI;

public static class AddApplication
{
    public static IServiceCollection AddApplicationExtensions(this IServiceCollection services)
    {
        services.AddMediatR( o =>
            o.RegisterServicesFromAssembly(typeof(Program).Assembly));
        
        services.AddScoped<ICacheService, CacheService>();
        
        return services;
    }
}