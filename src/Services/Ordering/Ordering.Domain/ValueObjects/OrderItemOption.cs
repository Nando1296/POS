namespace Ordering.Domain.ValueObjects;

public class OrderItemOption
{
    public string Name { get; private set; } = default!;
    public decimal AdditionalPrice { get; private set; }

    public OrderItemOption(string name, decimal additionalPrice)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Option name is required.");

        if(additionalPrice < 0)
            throw new ArgumentException("Additional price cannot be negative.");

        Name = name;
        AdditionalPrice = additionalPrice;
    }
}