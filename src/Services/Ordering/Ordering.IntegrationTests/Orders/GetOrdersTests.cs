using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Ordering.IntegrationTests.Orders;

[Collection("IntegrationTests")]
public class GetOrdersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient HttpClient;
    public GetOrdersTests(WebApplicationFactory<Program> factory)
    {
        HttpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrders_WhenDataBaseIsDown_ShouldReturnServiceUnavailable()
    {
        var response = await HttpClient.GetAsync("api/orders");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal("Database Error", problem.Title);
        Assert.Equal(503, problem.Status);
        Assert.Contains("unreachable", problem.Detail);
    }
}