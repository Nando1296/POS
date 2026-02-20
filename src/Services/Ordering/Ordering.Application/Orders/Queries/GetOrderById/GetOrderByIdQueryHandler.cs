using MediatR;
using Ordering.Application.DTOs.Responses;
using Ordering.Domain.Interfaces;
using ErrorOr;
using Ordering.Domain.Errors;

namespace Ordering.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ErrorOr<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<ErrorOr<OrderResponseDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id);
        
        if (order == null)
        {
            return DomainErrors.Orders.NotFound(request.Id);
        }

        var orderDetails = new OrderResponseDto
        (
            order.Id,
            order.TableNumber,
            order.Status.ToString(),
            order.CreatedAt,
            order.GetTotal(),
            order.Items.Select(item => new OrderItemResponseDto
            (
                item.ProductId,
                item.ProductName,
                item.UnitPrice,
                item.Quantity,
                item.Options.Select(option => new OrderItemOptionResponseDto
                (
                    option.Name,
                    option.AdditionalPrice
                )).ToList()
            )).ToList()
        );

        return orderDetails;
    }
}