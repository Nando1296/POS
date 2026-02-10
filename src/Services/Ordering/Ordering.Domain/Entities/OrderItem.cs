using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId  { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    private readonly List<OrderItemOption> _options = new();
    public IReadOnlyCollection<OrderItemOption> Options => _options.AsReadOnly();

    protected OrderItem() {}

    public OrderItem(
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        if (quantity <= 0)
        throw new ArgumentException("Quantity must be greater than zero.");

        if(UnitPrice < 0)
        throw new ArgumentException("Price cannot be negative.");

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
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