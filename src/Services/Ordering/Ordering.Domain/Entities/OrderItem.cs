using Ordering.Domain.ValueObjects;
using ErrorOr;
using Ordering.Domain.Errors;

namespace Ordering.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId  { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    private readonly List<OrderItemOption> _options = new();
    public IReadOnlyCollection<OrderItemOption> Options => _options.AsReadOnly();

    protected OrderItem() {}  //EF

    private OrderItem
    (
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity
    )
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static ErrorOr<OrderItem> Create(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
        {
            return DomainErrors.OrderItems.InvalidQuantity;
        }

        if(unitPrice < 0)
        {
            return DomainErrors.OrderItems.InvalidUnitPrice;
        }

        var orderItem = new OrderItem(productId, productName, unitPrice, quantity);
        return orderItem;
    }

    public void AddOption(OrderItemOption option)
    {
        _options.Add(option);
    }

    public decimal GetTotal()
    {
        var optionsTotal = _options.Sum( o => o.AdditionalPrice);
        return (UnitPrice + optionsTotal) * Quantity;
    }
}