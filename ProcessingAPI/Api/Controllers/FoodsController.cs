using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProcessingAPI.UseCases;

namespace ProcessingAPI.Controllers;

[Route(Consts.FoodsRoute)]
[ApiController]
public class FoodsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoodsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{clientId:guid}")]
    public async Task<IActionResult> GetFood(Guid clientId)
    {
        var result = await _mediator.Send(new GetFoodQuery(clientId));
        if(!result.IsSuccess)
        {
            return NotFound();
        }
        
        if (result.IsNoContent())
        {
            return Accepted();
        }
        
        return Ok(result.Value);
    }
    
}