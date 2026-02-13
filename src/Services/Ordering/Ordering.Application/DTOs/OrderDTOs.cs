namespace Ordering.Application.DTOs;

public record OrderItemOptionDto(string Name, decimal AdditionalPrice);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    List<OrderItemOptionDto> Options
);
