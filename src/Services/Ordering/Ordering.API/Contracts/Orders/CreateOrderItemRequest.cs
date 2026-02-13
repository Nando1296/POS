namespace Ordering.API.Contracts.Orders;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
}