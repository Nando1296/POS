using MediatR;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    int TableNumber,
    List<OrderItemDto> Items
) : IRequest<Guid>;
