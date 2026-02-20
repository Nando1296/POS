using MediatR;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;
using Ordering.Domain.Interfaces;
using ErrorOr;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ErrorOr<Guid>>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderResult = Order.Create(request.TableNumber);

        if (orderResult.IsError)
        {
            return orderResult.Errors;
        }

        var order = orderResult.Value;

        foreach (var itemDto in request.Items)
        {
            var itemResult = OrderItem.Create(
                itemDto.ProductId,
                itemDto.ProductName,
                itemDto.UnitPrice,
                itemDto.Quantity
            );

            if(itemResult.IsError)
            {
                return itemResult.Errors;
            }

            var orderItem = itemResult.Value;

            if (itemDto.Options != null)
            {
                foreach (var optionDto in itemDto.Options)
                {
                    var optionResult = OrderItemOption.Create(optionDto.Name, optionDto.AdditionalPrice);
                    if(optionResult.IsError)
                    {
                        return optionResult.Errors;
                    }

                    orderItem.AddOption(optionResult.Value);
                }
            }
            var addItemResult = order.AddItem(orderItem);
            if (addItemResult.IsError)
            {
                return addItemResult.Errors;
            }
        }

        await _orderRepository.AddAsync(order);

        return order.Id;
    }
}
