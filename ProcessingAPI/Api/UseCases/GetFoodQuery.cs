using Ardalis.Result;
using MediatR;

namespace ProcessingAPI.UseCases;

public record GetFoodQuery(Guid clientId) : IRequest<Result<GetFoodResponse>>;
