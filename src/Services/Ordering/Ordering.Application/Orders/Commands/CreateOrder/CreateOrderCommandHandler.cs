using MediatR;
using Ordering.Domain.Entities;
using Ordering.Domain.Interfaces;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.TableNumber);

        foreach (var itemDto in request.Items)
        {
            var orderItem = new OrderItem(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.UnitPrice,
                itemDto.Quantity
            );

            if (itemDto.Options != null)
            {
                foreach (var optionDto in itemDto.Options)
                {
                    var option = new OrderItemOption(optionDto.Name, optionDto.AdditionalPrice);
                    orderItem.AddOption(option);
                }
            }

            order.AddItem(orderItem);
        }

        await _orderRepository.AddAsync(order);

        return order.Id;
    }
}
