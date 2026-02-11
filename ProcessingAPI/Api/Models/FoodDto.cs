namespace ProcessingAPI.Models;

public class FoodForClientDto
{
    public Guid ClientId { get; set; }
    public string FoodName { get; set; }
    public DateTime ProcessingStartTime { get; set; }
}