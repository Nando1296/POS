using NSubstitute;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Interfaces;
using Ordering.Application.DTOs;
using Ordering.Domain.Entities;
using Xunit;
using FluentAssertions;
using Ordering.Domain.Errors;

namespace Ordering.UnitTests.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{

    private readonly IOrderRepository _mockOrderRepository;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _mockOrderRepository = Substitute.For<IOrderRepository>();
        _handler = new CreateOrderCommandHandler(_mockOrderRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnOrderId_WhenCommandIsValid()
    {
        var items = new List<OrderItemDto>
        {
            new OrderItemDto
            (
                Guid.NewGuid(),
                "Flat White",
                22.0m,
                2,
                new List<OrderItemOptionDto> { new OrderItemOptionDto("Soy milk", 2.0m) }
            ),
            new OrderItemDto
            (
                Guid.NewGuid(),
                "Latte",
                18.0m,
                1,
                new List<OrderItemOptionDto> { new OrderItemOptionDto("Extra shot", 5.0m) }
            )
        };

        var command = new CreateOrderCommand(5, items);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        await _mockOrderRepository.Received(1).AddAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenTableNumberIsInvalid()
    {
        var items = new List<OrderItemDto>
        {
            new OrderItemDto
            (
                Guid.NewGuid(),
                "Capuchino",
                20.0m,
                2,
                new List<OrderItemOptionDto>()
            )
        };
        var command = new CreateOrderCommand(-5, items);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(DomainErrors.Orders.InvalidTableNumber.Code);

        await _mockOrderRepository.DidNotReceive().AddAsync(Arg.Any<Order>());
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenOrderItemIsInvalid()
    {
        var items = new List<OrderItemDto>
        {
            new OrderItemDto
            (
                Guid.NewGuid(),
                "Cold Brew",
                10.0m,
                -2,
                new List<OrderItemOptionDto>()
            )
        };

        var command = new CreateOrderCommand(3, items);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(DomainErrors.OrderItems.InvalidQuantity.Code);

        await _mockOrderRepository.DidNotReceive().AddAsync(Arg.Any<Order>());
    }
}