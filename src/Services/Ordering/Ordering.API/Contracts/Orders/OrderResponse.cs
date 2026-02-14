namespace Ordering.API.Contracts.Orders;

public record OrderItemOptionResponse(string Name, decimal AdditionalPrice);

public record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    List<OrderItemOptionResponse> Options
);

public record OrderResponse(
    Guid Id,
    int TableNumber,
    string Status,
    DateTime CreatedAt,
    decimal TotalPrice,
    List<OrderItemResponse> Items
);