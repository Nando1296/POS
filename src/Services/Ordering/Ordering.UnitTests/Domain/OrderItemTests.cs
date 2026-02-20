using Ordering.Domain.Entities;
using Xunit;
using ErrorOr;
using FluentAssertions;
using Ordering.Domain.Errors;

namespace Ordering.UnitTests.Domain;

public class OrderItemTests
{
    [Fact]
    public void Create_ShouldReturnError_WhenQuantityIsZeroOrNegative()
    {
        int invalidQuantity = -2;

        var result = OrderItem.Create(Guid.NewGuid(), "Espresso", 15.0m, invalidQuantity);

        result.IsError.Should().BeTrue();

        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be(DomainErrors.OrderItems.InvalidQuantity.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenUnitPriceIsNegative()
    {
        decimal invalidUnitPrice = -5.0m;

        var result = OrderItem.Create(Guid.NewGuid(), "Espresso", invalidUnitPrice, 1);

        result.IsError.Should().BeTrue();

        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be(DomainErrors.OrderItems.InvalidUnitPrice.Code);
    }
}