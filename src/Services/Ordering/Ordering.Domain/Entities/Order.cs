using Ordering.Domain.Enums;
using Ordering.Domain.Exceptions;
using ErrorOr;
using Ordering.Domain.Errors;

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

    private Order(int tableNumber)
    {
        Id = Guid.NewGuid();
        TableNumber = tableNumber;
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    public static ErrorOr<Order> Create(int tableNumber)
    {
        if(tableNumber <= 0)
        {
            return DomainErrors.Orders.InvalidTableNumber;
        }

        return new Order(tableNumber);
    }

    public ErrorOr<Success> AddItem(OrderItem item)
    {
        if(Status != OrderStatus.Created)
        {
            return DomainErrors.Orders.InvalidState("Cannot add items to an order that is already in progress.");
        }

        _items.Add(item);
        return Result.Success;
    }

    public ErrorOr<Success> AcceptedOrder()
    {
        if(Status != OrderStatus.Created && Status != OrderStatus.Paid)
        {
            return DomainErrors.Orders.InvalidState("Only orders in 'Created' or 'Paid' status can be accepted.");
        }

        Status = OrderStatus.InPreparation;
        return Result.Success;
    }

    public ErrorOr<Success> MarkAsReady()
    {
        if(Status != OrderStatus.InPreparation)
        {
            return DomainErrors.Orders.InvalidState("Only orders in 'InPreparation' status can be marked as ready.");
        }

        Status = OrderStatus.Ready;
        return Result.Success;
    }

    public ErrorOr<Success> ServeOrder()
    {
        if(Status != OrderStatus.Ready)
        {
            return DomainErrors.Orders.InvalidState("Only orders in 'Ready' status can be served.");
        }

        Status = OrderStatus.Served;
        return Result.Success;
    }

    public ErrorOr<Success> MarkAsPaid()
    {
        if(Status != OrderStatus.Served && Status != OrderStatus.Created)
        {
            return DomainErrors.Orders.InvalidState("Only orders in 'Created' or 'Served' status can be marked as paid.");
        }

        Status = OrderStatus.Paid;
        return Result.Success;
    }

    public ErrorOr<Success> CancelOrder(string reason)
    {
        if(Status == OrderStatus.Paid)
        {
            return DomainErrors.Orders.InvalidState("Paid orders cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        return Result.Success;
    }

    public decimal GetTotal()
    {
        return _items.Sum(i => i.GetTotal());
    }
}