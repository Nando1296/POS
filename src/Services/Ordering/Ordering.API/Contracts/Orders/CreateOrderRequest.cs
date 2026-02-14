namespace Ordering.API.Contracts.Orders;

public record CreateOrderRequest(int TableNumber, List<OrderItemRequest> Items);

public record OrderItemRequest(
    Guid ProductId, 
    string ProductName, 
    decimal UnitPrice, 
    int Quantity, 
    List<OrderItemOptionRequest>? Options = null
);

public record OrderItemOptionRequest(string Name, decimal AdditionalPrice);