using Ardalis.Result;
using MediatR;
using ProcessingAPI.Models;
using ProcessingAPI.Services;

namespace ProcessingAPI.UseCases;

public class GetFoodHandler : IRequestHandler<GetFoodQuery, Result<GetFoodResponse>>
{
    private readonly IFoodRepository _foodRepository;
    private readonly ICacheService _cacheService;

    public GetFoodHandler(IFoodRepository foodRepository, ICacheService cacheService)
    {
        _foodRepository = foodRepository;
        _cacheService = cacheService;
    }
    public async Task<Result<GetFoodResponse>> Handle(GetFoodQuery request, CancellationToken cancellationToken)
    {
        var cachedFood = _cacheService.Get(request.clientId);
        if (cachedFood.IsNotFound())
        {
            var result = _foodRepository.GetRandomFood();
            var foodToCache = new FoodForClientDto() 
            {
                ClientId = request.clientId,
                FoodName = result.Name,
                ProcessingStartTime = DateTime.Now
            };
            
            _cacheService.Set(request.clientId, foodToCache);
            
            return Result.NoContent();
        }

        if(DateTime.Now - cachedFood.Value.ProcessingStartTime < TimeSpan.FromSeconds(Consts.TimeOfCalculation))
        {
            return Result.NoContent();
        }

        return Result.Success(new GetFoodResponse()
        {
            FoodForCLientResponse = cachedFood.Value
        });
    }
}