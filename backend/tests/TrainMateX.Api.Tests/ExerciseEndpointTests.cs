using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TrainMateX.Api.Dtos;

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

        var benchPress = Assert.Single(exercises, exercise => exercise.Id == "bench-press");

        Assert.Equal("Bench Press", benchPress.Name);
        Assert.Equal("Chest", benchPress.MuscleGroup);
        Assert.Equal("Intermediate", benchPress.DifficultyLevel);
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
        Assert.Equal(id, exercise.Id);
        Assert.Equal("Bench Press", exercise.Name);

        Assert.False(string.IsNullOrWhiteSpace(exercise.Description));
        Assert.NotEmpty(exercise.Instructions);
        Assert.Equal("Chest", exercise.MuscleGroup);

        Assert.False(string.IsNullOrWhiteSpace(exercise.Equipment));

        Assert.Equal("Intermediate", exercise.DifficultyLevel);
    }

    [Fact]
    public async Task GetExerciseById_ShouldReturn404NotFound_WhenExerciseMissing()
    {
        var id = "missing-exercise";
        var response = await _client.GetAsync($"/api/exercises/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateExercise_ShouldReturn201_AndBeAvailableThroughGet()
    {
        var request = new SaveExerciseRequest(
            Name: "Overhead Press Integration Test",
            Description: "A compound upper-body exercise performed with a barbell.",
            Instructions:
            [
                "Stand with the bar at shoulder height.",
                 "Brace your core.",
                 "Press the bar overhead until your arms are extended."
            ],
            MuscleGroup: "Shoulders",
            Equipment: "Barbell",
            DifficultyLevel: "Intermediate");


        var postResponse = await _client.PostAsJsonAsync("/api/exercises", request);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var location = postResponse.Headers.Location;

        Assert.NotNull(location);
        Assert.Equal($"/api/exercises/overhead-press-integration-test", location.ToString());

        var createdExercise = await postResponse.Content.ReadFromJsonAsync<ExerciseDto>();

        Assert.NotNull(createdExercise);
        Assert.Equal("overhead-press-integration-test", createdExercise.Id);

        var getResponse = await _client.GetAsync(location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetchedExercise = await getResponse.Content.ReadFromJsonAsync<ExerciseDto>();

        Assert.NotNull(fetchedExercise);
        Assert.Equal(createdExercise.Id, fetchedExercise.Id);
        Assert.Equal(request.Name, fetchedExercise.Name);
    }

    [Fact]
    public async Task CreateExercise_ShouldReturn400_WhenValidationFails()
    {
        var request = new SaveExerciseRequest(
            Name: "",
            Description: "A compound upper-body exercise performed with a barbell.",
            Instructions: [],
            MuscleGroup: "Shoulders",
            Equipment: "Barbell",
            DifficultyLevel: "Intermediate");

        var response = await _client.PostAsJsonAsync("/api/exercises", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var validationProblem = await response.Content
            .ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.NotNull(validationProblem);

        Assert.Contains("Name", validationProblem.Errors.Keys);
        Assert.Contains("Name is required.", validationProblem.Errors["Name"]);
    }

    [Fact]
    public async Task CreateExercise_ShouldReturn409_WhenConflictingName()
    {
        var request = new SaveExerciseRequest(
            Name: "Bench Press",
            Description: "A compound upper-body exercise performed with a barbell.",
            Instructions: ["Lie on the bench with your eyes under the bar."],
            MuscleGroup: "Chest",
            Equipment: "Barbell",
            DifficultyLevel: "Intermediate");

        var response = await _client.PostAsJsonAsync("/api/exercises", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var errors = await response.Content
            .ReadFromJsonAsync<Dictionary<string, string[]>>();

        Assert.NotNull(errors);
        Assert.Contains("Name", errors.Keys);
        Assert.Contains("An exercise with this name already exists.", errors["Name"]);
    }
}
