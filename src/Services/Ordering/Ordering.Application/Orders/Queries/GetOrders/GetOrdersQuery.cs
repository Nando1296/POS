using MediatR;
using ErrorOr;
using Ordering.Application.DTOs.Responses;

namespace Ordering.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null
) : IRequest<ErrorOr<IReadOnlyList<OrderSummaryResponseDto>>>;