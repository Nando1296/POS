using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Ordering.IntegrationTests.Base;

namespace Ordering.IntegrationTests.Orders;

[Collection("IntegrationTests")]
public class OrderLifecycleTests : BaseIntegrationTest
{
    public OrderLifecycleTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAndRetrieveOrder_Lifecycle_ShouldPersistAndMatchData()
    {
        var productId = Guid.NewGuid();

        var command = new
        {
            TableNumber = 12,
            Items = new[]
            {
                new
                {
                    ProductId = productId,
                    ProductName = "Flatwhite",
                    UnitPrice = 22.00m,
                    Quantity = 2,
                    Options = new[]
                    {
                        new { Name = "Leche deslactosada", AdditionalPrice = 2}
                    }
                }
            }
        };

        var postResponse = await HttpClient.PostAsJsonAsync("api/orders", command);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var postResponseData = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
        var orderIdString = postResponseData.GetProperty("orderId").GetString();
        Assert.True(Guid.TryParse(orderIdString, out var orderId));

        var getResponse = await HttpClient.GetAsync($"api/orders/{orderId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getResponseData = await getResponse.Content.ReadFromJsonAsync<JsonElement>();

        var retrievedTableNumber = getResponseData.GetProperty("tableNumber").GetInt32();
        Assert.Equal(12, retrievedTableNumber);

        var itemsArray = getResponseData.GetProperty("items");
        Assert.Equal(1, itemsArray.GetArrayLength());

        var totalPrice = getResponseData.GetProperty("totalPrice").GetDecimal();
        Assert.Equal(48.00m, totalPrice);
    }
}