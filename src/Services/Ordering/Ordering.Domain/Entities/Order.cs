using Ordering.Domain.Enums;

namespace Ordering.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public int TableNumber { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() {}

    public Order(int tableNumber)
    {
        if(tableNumber <=0)
        throw new ArgumentException("Table number must be valid.");

        Id = Guid.NewGuid();
        TableNumber = tableNumber;
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        if(Status != OrderStatus.Created)
            throw new ArgumentException("Cannot add items to an order that is already in progress.");

        _items.Add(item);
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if(Status == OrderStatus.Paid)
            throw new ArgumentException("Paid orders cannot change status.");

        Status = newStatus;
    }

    public decimal GetTotal()
    {
        return _items.Sum(i => i.GetTotal());
    }
}