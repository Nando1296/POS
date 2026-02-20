using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Ordering.IntegrationTests.Base;

namespace Ordering.IntegrationTests.Orders;

[Collection("IntegrationTests")]
public class CreateOrderTests : BaseIntegrationTest
{
    public CreateOrderTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldReturnCreated()
    {
        var command = new
        {
            TableNumber = 5,
            Items = new []
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Café Americano",
                    UnitPrice = 15.00m,
                    Quantity = 2,
                    Options = new []
                    {
                        new { Name = "Sin Azúcar", AdditionalPrice = 0.0m},
                        new { Name = "Con Crema", AdditionalPrice = 3.0m}
                    }
                }
            }
        };

        var response = await HttpClient.PostAsJsonAsync("api/orders", command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();

        var orderIdString = responseData.GetProperty("orderId").GetString();
        Assert.True(Guid.TryParse(orderIdString, out var orderId));
        Assert.NotEqual(Guid.Empty, orderId);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidTableNumber_ShouldReturnBadRequest()
    {
        var command = new
        {
            TableNumber = -7,
            Items = new []
            {
                new
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Latte",
                    UnitPrice = 18.00m,
                    Quantity = 2,
                    Options = new []
                    {
                        new { Name = "Leche Deslactosada Light" , AdditionalPrice = 2.0m },
                        new { Name =  "Sin azucar", AdditionalPrice = 0.0m }
                    }
                }
            }
        };

        var response = await HttpClient.PostAsJsonAsync("api/orders", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(400, problemDetails.Status);

        var hasTableError = problemDetails.Errors.Keys.Any( k => k.Equals("TableNumber", StringComparison.OrdinalIgnoreCase));
        Assert.True(hasTableError, "Expected a validation error for TableNumber.");
    }

    [Fact]
    public async Task CreateOrder_WithInvalidItemData_ShouldReturnBadRequest()
    {
        var command = new
        {
            TableNumber = 5,
            Items = Array.Empty<object>()
        };

        var response = await HttpClient.PostAsJsonAsync("api/orders", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);

        var hasItemError = problemDetails.Errors.Keys.Any( k => k.Equals("Items", StringComparison.OrdinalIgnoreCase));
        Assert.True(hasItemError, "Expected a validation error for Items list.");
    }
}