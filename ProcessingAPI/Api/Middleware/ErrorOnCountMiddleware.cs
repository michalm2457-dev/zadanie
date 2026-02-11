namespace ProcessingAPI.Middleware;

public class ErrorOnCountMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorOnCountMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        int currentCount = Interlocked.Increment(ref Flags._requestCount);

        if (Flags._failOnNextRequest)
        {
            Flags._failOnNextRequest = false;
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return;
        }
        
        if (currentCount % Consts.RequestFailedOnBigCount == 0)
        {   
            Flags._failOnNextRequest = true;
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return;
        }
        
        if (currentCount % Consts.RequestFailedOnSmallCount == 0)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return;
        }
        

        await _next(context);
    }
    
    
}
