using Ardalis.Result;
using ProcessingAPI.Models;

namespace ProcessingAPI.Services;

public interface ICacheService
{
    Result<FoodForClientDto> Get(Guid clientId);
    void Set(Guid clientId, FoodForClientDto foodForClientDto);
}