using FluentAssertions;
using NSubstitute;
using Ordering.Application.Orders.Queries.GetOrders;
using Ordering.Domain.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Domain.Errors;
using Xunit;

namespace Ordering.UnitTests.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandlerTests
{
    private readonly IOrderRepository _mockOrderRepository;
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _mockOrderRepository = Substitute.For<IOrderRepository>();
        _handler = new GetOrdersQueryHandler(_mockOrderRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoOrdersExist()
    {
        _mockOrderRepository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Order>>(new List<Order>()));

        var query = new GetOrdersQuery(1, 10, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnMappedOrderSummaries_WhenOrdersExist()
    {
        var order1 = Order.Create(1).Value;
        var order2 = Order.Create(2).Value;

        order1.AddItem(OrderItem.Create(Guid.NewGuid(), "Espresso", 10.0m, 1).Value);
        order2.AddItem(OrderItem.Create(Guid.NewGuid(), "Latte", 15.0m, 2).Value);

        var orderList = new List<Order> { order1, order2 };

        _mockOrderRepository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Order>>(orderList));
        

        var query = new GetOrdersQuery(1, 10, null);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);

        result.Value[0].TableNumber.Should().Be(1);
        result.Value[0].Total.Should().Be(10.0m);

        result.Value[1].TableNumber.Should().Be(2);
        result.Value[1].Total.Should().Be(30.0m);
    }
}
