using Ardalis.Result;
using Microsoft.Extensions.Caching.Memory;
using ProcessingAPI.Models;

namespace ProcessingAPI.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    public CacheService(IMemoryCache memoryCache)
    {
        _cache = memoryCache;
    }
    
    public Result<FoodForClientDto> Get(Guid clientId)
    {
        _cache.TryGetValue(clientId, out FoodForClientDto? foodForClientDto);
        
        if (foodForClientDto is null)
        {
            return Result<FoodForClientDto>.NotFound();
        }
        
        return Result.Success(foodForClientDto);
    }

    public void Set(Guid clientId, FoodForClientDto foodForClientDto)
    {
        _cache.Set(clientId, foodForClientDto, new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration =  DateTimeOffset.Now.AddSeconds(Consts.CacheExpirationTime)
        });
    }
}