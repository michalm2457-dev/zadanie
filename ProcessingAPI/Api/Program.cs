using ProcessingAPI.DI;
using ProcessingAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddInfrastructureExtensions();
builder.Services.AddApplicationExtensions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors(policy => policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseMiddleware<ErrorOnCountMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
