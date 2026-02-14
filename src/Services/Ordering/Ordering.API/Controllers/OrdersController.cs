using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Contracts.Orders;
using Ordering.Application.Orders.Queries.GetOrderById;
using Ordering.Application.Orders.Commands.CreateOrder; 
using Ordering.Application.DTOs;
using Ordering.Domain.Exceptions;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
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

        return Ok(new { 
            Id = result, 
            Message = "Order stored in Database!" 
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _sender.Send(query);

        if (result == null)
        {
            return NotFound(new { Message = $"Order with ID {id} not found." });
        }

        var order = new OrderResponse(
            result.Id,
            result.TableNumber,
            result.Status,
            result.CreatedAt,
            result.TotalPrice,
            result.Items.Select(i => new OrderItemResponse(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.Options.Select(o => new OrderItemOptionResponse(o.Name, o.AdditionalPrice)).ToList()
            )).ToList()
        );

        return Ok(order);
    }
}