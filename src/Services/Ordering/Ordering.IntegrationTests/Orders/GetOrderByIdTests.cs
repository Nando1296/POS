using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Ordering.IntegrationTests.Base;

namespace Ordering.IntegrationTests.Orders;

[Collection("IntegrationTests")]
public class GetOrderByIdTests: BaseIntegrationTest
{
    public GetOrderByIdTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetOrderById_WithNotExistingId_ShouldReturnNotFound()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await HttpClient.GetAsync($"api/orders/{nonExistingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(404, problemDetails.Status);
    }
}