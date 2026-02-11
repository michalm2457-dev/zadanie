using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ProcessingAPI;
using ProcessingAPI.UseCases;

namespace Api.Tests;

public class ProcessingApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    [Fact]
    public async Task ErrorOnCountMiddleware_Every5thRequest_Returns503ServiceUnavailable()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();

        for (int i = 1; i <= Consts.RequestFailedOnSmallCount; i++)
        {
            var response = await CallApiGetAsync(clientId);
            if (i == Consts.RequestFailedOnSmallCount)
                Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            else
                Assert.NotEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }

    [Fact]
    public async Task ErrorOnCountMiddleware_Every10thRequest_ReturnsTwoConsecutive503()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();

        for (int i = 1; i <= Consts.RequestFailedOnBigCount + 1; i++)
        {
            var response = await CallApiGetAsync(clientId);
            if (i == Consts.RequestFailedOnBigCount || i == Consts.RequestFailedOnBigCount + 1)
                Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            else if (i == Consts.RequestFailedOnSmallCount)
                Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            else
                Assert.NotEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }

    [Fact]
    public async Task FirstRequest_ReturnsAccepted()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();
        var response = await CallApiGetAsync(clientId);
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task Pooling_ReturnsAccepted()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();

        var first = await CallApiGetAsync(clientId);
        Assert.Equal(HttpStatusCode.Accepted, first.StatusCode);
        
        var second = await CallApiGetAsync(clientId);
        Assert.Equal(HttpStatusCode.Accepted, second.StatusCode);
    }


    [Fact]
    public async Task ProcessingComplete_ReturnsOk()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();

        var start = await CallApiGetAsync(clientId);
        Assert.Equal(HttpStatusCode.Accepted, start.StatusCode);

        await Task.Delay(TimeSpan.FromSeconds(Consts.TimeOfCalculation + 1));

        var result = await CallApiGetAsync(clientId);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var body = await result.Content.ReadFromJsonAsync<GetFoodResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotNull(body.FoodForCLientResponse);
        Assert.Equal(clientId, body.FoodForCLientResponse.ClientId);
        Assert.False(string.IsNullOrWhiteSpace(body.FoodForCLientResponse.FoodName));
    }

    [Fact]
    public async Task DifferentClients_IndependentResponses()
    {
        ResetRequestCounter();
        var clientA = Guid.NewGuid();
        var clientB = Guid.NewGuid();

        var respA = await CallApiGetAsync(clientA);
        var respB = await CallApiGetAsync(clientB);

        await Task.Delay(TimeSpan.FromSeconds(Consts.TimeOfCalculation + 1));

        var resultA = await CallApiGetAsync(clientA);
        var resultB = await CallApiGetAsync(clientB);

        var bodyA = await resultA.Content.ReadFromJsonAsync<GetFoodResponse>(JsonOptions);
        var bodyB = await resultB.Content.ReadFromJsonAsync<GetFoodResponse>(JsonOptions);

        Assert.Equal(clientA, bodyA!.FoodForCLientResponse.ClientId);
        Assert.Equal(clientB, bodyB!.FoodForCLientResponse.ClientId);
    }
    
    [Fact]
    public async Task CacheCheck_ClientGetsCachedResponse()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();

        await CallApiGetAsync(clientId);
        await Task.Delay(TimeSpan.FromSeconds(Consts.TimeOfCalculation + 1));

        var first = await CallApiGetAsync(clientId);
        var second = await CallApiGetAsync(clientId);

        var bodyFirst = await first.Content.ReadFromJsonAsync<GetFoodResponse>(JsonOptions);
        var bodySecond = await second.Content.ReadFromJsonAsync<GetFoodResponse>(JsonOptions);

        Assert.Equal(bodyFirst!.FoodForCLientResponse.FoodName,bodySecond!.FoodForCLientResponse.FoodName);
        Assert.Equal(bodyFirst.FoodForCLientResponse.ProcessingStartTime,bodySecond.FoodForCLientResponse.ProcessingStartTime);
    }

    [Fact]
    public async Task Cache_Expires_AfterTime()
    {
        ResetRequestCounter();
        var clientId = Guid.NewGuid();

        await CallApiGetAsync(clientId);
        await Task.Delay(TimeSpan.FromSeconds(Consts.TimeOfCalculation + 1));
        await Task.Delay(TimeSpan.FromSeconds(Consts.CacheExpirationTime + 1));

        var afterExpiry = await CallApiGetAsync(clientId);
        Assert.Equal(HttpStatusCode.Accepted, afterExpiry.StatusCode);
    }
    
    public ProcessingApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    private async Task<HttpResponseMessage> CallApiGetAsync(Guid clientId)
    {
        return await _client.GetAsync($"/api/foods/{clientId}");
    }

    private static void ResetRequestCounter()
    {
        Flags._requestCount = 0;
        Flags._failOnNextRequest = false;
    }
}
