using FluentAssertions;
using NSubstitute;
using Ordering.Application.Orders.Queries.GetOrderById;
using Ordering.Domain.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Domain.Errors;

namespace Ordering.UnitTests.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    private readonly IOrderRepository _mockOrderRepository;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _mockOrderRepository = Substitute.For<IOrderRepository>();
        _handler = new GetOrderByIdQueryHandler(_mockOrderRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        _mockOrderRepository.GetByIdAsync(orderId).Returns(Task.FromResult<Order?>(null));

        var query = new GetOrderByIdQuery(orderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(DomainErrors.Orders.NotFound(orderId).Code);

        await _mockOrderRepository.Received(1).GetByIdAsync(orderId);
    }

    [Fact]
    public async Task Handle_Should_ReturnOrderResponseDto_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var tableNumber = 5;

        var orderResult = Order.Create(tableNumber);
        var validOrder = orderResult.Value;

        var itemResult =  OrderItem.Create(Guid.NewGuid(), "Flat White", 22.0m, 2);
        validOrder.AddItem(itemResult.Value);

        _mockOrderRepository.GetByIdAsync(orderId).Returns(Task.FromResult<Order?>(validOrder));

        var query = new GetOrderByIdQuery(orderId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.TableNumber.Should().Be(tableNumber);

        result.Value.TotalPrice.Should().Be(44.0m);
        result.Value.Items.Should().HaveCount(1);

        await _mockOrderRepository.Received(1).GetByIdAsync(orderId);

    }
}