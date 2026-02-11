using ProcessingAPI.Services;

namespace ProcessingAPI.DI;

public static class AddInfrastructure
{
    public static IServiceCollection AddInfrastructureExtensions(this IServiceCollection services)
    {
        services.AddScoped<IFoodRepository, FoodListRepository>();
        
        return  services;
    }
}