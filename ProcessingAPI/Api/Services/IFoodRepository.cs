using ProcessingAPI.Models;

namespace ProcessingAPI.Services;

public interface IFoodRepository
{
    Food GetRandomFood();
}