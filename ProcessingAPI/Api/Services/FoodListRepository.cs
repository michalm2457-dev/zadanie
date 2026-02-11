using ProcessingAPI.Models;

namespace ProcessingAPI.Services;

public class FoodListRepository : IFoodRepository
{
    private readonly List<Food> _foodList = new();
    public FoodListRepository()
    {
        _foodList.Add(new Food
        {
            Name = "Apple",
        });
        _foodList.Add(new Food
        {
            Name = "Banana",
        });
        _foodList.Add(new Food
        {
            Name = "Orange",
        });
    }

    public Food GetRandomFood()
    {
        var random = new Random();
        return _foodList[random.Next(0, _foodList.Count)];
    }
}
