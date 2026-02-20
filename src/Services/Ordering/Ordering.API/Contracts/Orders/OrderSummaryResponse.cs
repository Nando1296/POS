namespace Ordering.API.Contracts.Orders;

public record OrderSummaryResponse(
    Guid Id,
    int TableNumber,
    string Status,
    DateTime CreatedAt,
    decimal Total
);