using MediatR;
using Ordering.Application.DTOs;
using ErrorOr;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    int TableNumber,
    List<OrderItemDto> Items
) : IRequest<ErrorOr<Guid>>;
