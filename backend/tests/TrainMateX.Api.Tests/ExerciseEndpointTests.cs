using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TrainMateX.Api.DTOs;

namespace TrainMateX.Api.Tests;

public class ExerciseEndpointTests : IClassFixture<TrainMateXApiFactory>
{
    private readonly HttpClient _client;

    public ExerciseEndpointTests(TrainMateXApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetExercises_ShouldReturnExerciseList()
    {
        var response = await _client.GetAsync("/api/exercises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercises = await response.Content.ReadFromJsonAsync<List<ExerciseListDto>>();

        Assert.NotNull(exercises);
        Assert.NotEmpty(exercises);

        var benchPress = Assert.Single(exercises, exercise => exercise.id == "bench-press");

        Assert.Equal("Bench Press", benchPress.name);
        Assert.Equal("Chest", benchPress.muscleGroup);
        Assert.Equal("Intermediate", benchPress.difficultyLevel);
    }

    [Fact]
    public async Task GetExercises_ShouldReturnOnlyListFields()
    {
        var response = await _client.GetAsync("/api/exercises");

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var firstExercise = document.RootElement.EnumerateArray().First();

        Assert.True(firstExercise.TryGetProperty("id", out _));
        Assert.True(firstExercise.TryGetProperty("name", out _));
        Assert.True(firstExercise.TryGetProperty("muscleGroup", out _));

        Assert.True(firstExercise.TryGetProperty("difficultyLevel", out _));

        Assert.Equal(4, firstExercise.EnumerateObject().Count());

        Assert.False(firstExercise.TryGetProperty("description", out _));
        Assert.False(firstExercise.TryGetProperty("instructions", out _));
        Assert.False(firstExercise.TryGetProperty("equipment", out _));
    }

    [Fact]
    public async Task GetExerciseById_ShouldReturnExerciseDetails_WhenExerciseExists()
    {
        var id = "bench-press";

        var response = await _client.GetAsync($"/api/exercises/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercise = await response.Content.ReadFromJsonAsync<ExerciseDto>();

        Assert.NotNull(exercise);
        Assert.Equal(id, exercise.id);
        Assert.Equal("Bench Press", exercise.name);

        Assert.False(string.IsNullOrWhiteSpace(exercise.description));
        Assert.NotEmpty(exercise.instructions);
        Assert.Equal("Chest", exercise.muscleGroup);

        Assert.False(string.IsNullOrWhiteSpace(exercise.equipment));

        Assert.Equal("Intermediate", exercise.difficultyLevel);
    }

    [Fact]
    public async Task GetExerciseById_ShouldReturn404NotFound_WhenExerciseMissing()
    {
        var id = "missing-exercise";
        var response = await _client.GetAsync($"/api/exercises/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
