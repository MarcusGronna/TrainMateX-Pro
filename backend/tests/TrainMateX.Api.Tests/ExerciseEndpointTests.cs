using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

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

    [Fact]
    public async Task GetExerciseById_ShouldReturn200OK_WhenExerciseExists()
    {
        var id = "bench-press";
        var response = await _client.GetAsync($"/exercises/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercise = await response.Content.ReadFromJsonAsync<Exercise>();
        Assert.NotNull(exercise);
        Assert.Equal(id, exercise.Id);
    }

    [Fact]
    public async Task GetExerciseById_ShouldReturn404NotFound_WhenExerciseMissing()
    {
        var id = "missing-exercise";
        var response = await _client.GetAsync($"/exercises/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
