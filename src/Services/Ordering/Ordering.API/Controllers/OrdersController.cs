using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Contracts.Orders;
using Ordering.Application.Orders.Queries.GetOrderById;
using Ordering.Application.Orders.Queries.GetOrders;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Application.DTOs;
using Ordering.Application.DTOs.Responses;

namespace Ordering.API.Controllers;

[Route("api/orders")]
public class OrdersController : ApiController
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.TableNumber,
            request.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.Options?.Select(o => new OrderItemOptionDto(o.Name, o.AdditionalPrice)).ToList() ?? []
            )).ToList()
        );

        var result = await _sender.Send(command);

        return result.Match(
            orderId => StatusCode(StatusCodes.Status201Created, new { OrderId = orderId, Message = "Order created successfully."}),
            errors => Problem(errors)
        );
    }   

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _sender.Send(query);

        return result.Match(
            orderDto => Ok(MapToOrderResponse(orderDto)),
            errors => Problem(errors)
        );
    }

    private static OrderResponse MapToOrderResponse(OrderResponseDto orderResponseDto)
    {
        var order = new OrderResponse(
            orderResponseDto.Id,
            orderResponseDto.TableNumber,
            orderResponseDto.Status,
            orderResponseDto.CreatedAt,
            orderResponseDto.TotalPrice,
            orderResponseDto.Items.Select(i => new OrderItemResponse(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.Options.Select(o => new OrderItemOptionResponse(o.Name, o.AdditionalPrice)).ToList()
            )).ToList()
        );

        return order;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null, CancellationToken cancellationToken = default)
    {
        var query = new GetOrdersQuery(pageNumber, pageSize, status);
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            orders => Ok(orders.Select(o => new OrderSummaryResponse(
                o.Id,
                o.TableNumber,
                o.Status,
                o.CreatedAt,
                o.Total
            )).ToList()),
            errors => Problem(errors)
        );
    }

}