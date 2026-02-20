namespace Ordering.Application.DTOs.Responses;

public record OrderItemOptionResponseDto(string Name, decimal AdditionalPrice);

public record OrderItemResponseDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    List<OrderItemOptionResponseDto> Options
);

public record OrderResponseDto(
    Guid Id,
    int TableNumber,
    string Status,
    DateTime CreatedAt,
    decimal TotalPrice,
    List<OrderItemResponseDto> Items
);