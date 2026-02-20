using MediatR;
using ErrorOr;
using Ordering.Application.DTOs.Responses;
using Ordering.Domain.Interfaces;
using Ordering.Domain.Errors;

namespace Ordering.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ErrorOr<IReadOnlyList<OrderSummaryResponseDto>>>
{
    private readonly IOrderRepository _orderRespository;

    public GetOrdersQueryHandler(IOrderRepository orderRespository)
    {
        _orderRespository = orderRespository;
    }

    public async Task<ErrorOr<IReadOnlyList<OrderSummaryResponseDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRespository.GetAllAsync(request.PageNumber, request.PageSize, request.Status, cancellationToken);

        var orderSummaries = orders.Select(order => new OrderSummaryResponseDto
        (
            order.Id,
            order.TableNumber,
            order.Status.ToString(),
            order.CreatedAt,
            order.GetTotal()
        )).ToList();

        return orderSummaries;
    }
}

