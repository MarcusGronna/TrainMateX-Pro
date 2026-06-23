using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace TrainMateX.Api.Tests;

public class ExerciseEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExerciseEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetExercises_ShouldReturn200OK_WhenExercisesExist()
    {
        var response = await _client.GetAsync($"/exercises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
