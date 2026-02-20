using ErrorOr;
using Ordering.Domain.Errors;
namespace Ordering.Domain.ValueObjects;

public class OrderItemOption
{
    public string Name { get; private set; } = default!;
    public decimal AdditionalPrice { get; private set; }

    private OrderItemOption(string name, decimal additionalPrice)
    {
        Name = name;
        AdditionalPrice = additionalPrice;
    }

    public static ErrorOr<OrderItemOption> Create(string name, decimal additionalPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainErrors.OrderItemOptions.InvalidName;
        }

        if (additionalPrice < 0)
        {
            return DomainErrors.OrderItemOptions.InvalidAdditionalPrice;
        }

        return new OrderItemOption(name, additionalPrice);
    }
}