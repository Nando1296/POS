using FluentValidation;

namespace Ordering.Application.Orders.Commands.CreateOrder;
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.TableNumber)
            .GreaterThan(0)
            .WithMessage("Table number must be positive.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items)
            .ChildRules(items =>
            {
                items.RuleFor(x => x.ProductId)
                    .NotEmpty()
                    .WithMessage("Product ID is required.");

                items.RuleFor(x => x.ProductName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Product name must not exceed 100 characters.");

                items.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero.");

                items.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than zero.");
                    
            });
    }
}