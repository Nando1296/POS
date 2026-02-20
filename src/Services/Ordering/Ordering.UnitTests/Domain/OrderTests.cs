using ErrorOr;
using FluentAssertions;
using Ordering.Domain.Entities;
using Xunit;
using Ordering.Domain.Errors;

namespace Ordering.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ShoulReturnOrder_WhenTableNumberIsPositive()
    {
        int validTableNumber = 1;
        
        var result = Order.Create(validTableNumber);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.TableNumber.Should().Be(validTableNumber);
        result.Value.Status.ToString().Should().Be("Created");
    }

    [Fact]
    public void Create_ShouldReturnValidationError_WhenTableNumberIsNegative()
    {
        int invalidTableNumber = -1;

        var result = Order.Create(invalidTableNumber);

        result.IsError.Should().BeTrue();

        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be(DomainErrors.Orders.InvalidTableNumber.Code);
    }

    [Fact]
    public void Create_ShouldReturnValidationError_WhenTableNumberIsZero()
    {
        int invalidTableNumber = 0;

        var result = Order.Create(invalidTableNumber);

        result.IsError.Should().BeTrue();

        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be(DomainErrors.Orders.InvalidTableNumber.Code);
    }
}