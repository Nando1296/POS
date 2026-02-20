namespace Ordering.Application.Orders.Queries.GetOrders;

public record OrderSummaryResponseDto(
    Guid Id,
    int TableNumber,
    string Status,
    DateTime CreatedAt,
    decimal Total
);