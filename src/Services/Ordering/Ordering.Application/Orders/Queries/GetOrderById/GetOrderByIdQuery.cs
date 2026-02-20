using MediatR;
using Ordering.Application.DTOs.Responses;
using ErrorOr;

namespace Ordering.Application.Orders.Queries.GetOrderById;
public record GetOrderByIdQuery(Guid Id) : IRequest<ErrorOr<OrderResponseDto>>;
