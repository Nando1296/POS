using FluentValidation;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(50)
            .WithMessage("Page size must be less than or equal to 100.");

        RuleFor(x => x.Status)
            .Must(BeAValidStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be one of the following: Created, InPreparation, Ready, Served, Paid, Cancelled.");
    }

    private bool BeAValidStatus(string? status)
    {
        return Enum.TryParse<OrderStatus>(status, true, out _);
    }
}