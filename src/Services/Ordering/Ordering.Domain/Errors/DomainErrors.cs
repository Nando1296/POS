using ErrorOr;

namespace Ordering.Domain.Errors;

public static partial class DomainErrors
{
    public static class Orders
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            code: "Order.NotFound",
            description: $"Order with ID {id} was not found."
        );

        public static Error InvalidTableNumber => Error.Validation(
            code: "Order.InvalidTableNumber",
            description: "Table number must be valid and positive."
        );

        public static Error InvalidState(string description) => Error.Conflict(
            code: "Order.InvalidState",
            description: description
        );
    }

    public static class OrderItems
    {
        public static Error InvalidQuantity => Error.Validation(
            code: "OrderItem.InvalidQuantity",
            description: "Order item quantity must be greater than zero."
        );

        public static Error InvalidUnitPrice => Error.Validation(
            code: "OrderItem.InvalidUnitPrice",
            description: "Order item unit price cannot be negative."
        );
    }

    public static class OrderItemOptions
    {
        public static Error InvalidName => Error.Validation(
            code: "OrderItemOption.InvalidName",
            description: "Order item option name is required."
        );

        public static Error InvalidAdditionalPrice => Error.Validation(
            code: "OrderItemOption.InvalidAdditionalPrice",
            description: "Order item option additional price cannot be negative."
        );
    }
}