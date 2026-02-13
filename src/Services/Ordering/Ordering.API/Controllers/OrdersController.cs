using MediatR; // <--- Necesario
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Contracts.Orders;
using Ordering.Application.Orders.Commands.CreateOrder; // <--- Necesario

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender; // Inyectamos el mediador

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.TableNumber,
            request.Items.Select(i => new Ordering.Application.DTOs.OrderItemDto(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.Options.Select(o => new Ordering.Application.DTOs.OrderItemOptionDto(o.Name, o.AdditionalPrice)).ToList()
            )).ToList()
        );

        var result = await _sender.Send(command);

        return Ok(new { 
            Id = result, 
            Message = "Order stored in Database!" 
        });
    }
}