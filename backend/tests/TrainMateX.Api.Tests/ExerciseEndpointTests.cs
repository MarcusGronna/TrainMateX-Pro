using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TrainMateX.Api.Dtos;

namespace TrainMateX.Api.Tests;

public class ExerciseEndpointTests : IClassFixture<TrainMateXApiFactory>
{
    private readonly TrainMateXApiFactory _factory;
    private readonly HttpClient _client;

    public ExerciseEndpointTests(TrainMateXApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetExercises_ShouldReturnExerciseList()
    {
        await _factory.ResetDatabaseAsync();

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
        await _factory.ResetDatabaseAsync();

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
        await _factory.ResetDatabaseAsync();

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
        await _factory.ResetDatabaseAsync();

        var id = "missing-exercise";
        var response = await _client.GetAsync($"/api/exercises/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateExercise_ShouldReturn201_AndBeAvailableThroughGet()
    {
        await _factory.ResetDatabaseAsync();

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
        await _factory.ResetDatabaseAsync();

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
        await _factory.ResetDatabaseAsync();

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

    [Fact]
    public async Task CreateExercise_ShouldReturn400_WhenDifficultyLevelIsInvalid()
    {
        await _factory.ResetDatabaseAsync();

        var request = new SaveExerciseRequest(
            Name: "Invalid Difficulty Integration Test",
            Description: "A request with an invalid difficulty level.",
            Instructions: ["Perform the exercise."],
            MuscleGroup: "Shoulders",
            Equipment: "Barbell",
            DifficultyLevel: "Impossible");

        var response = await _client.PostAsJsonAsync("/api/exercises", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var validationProblem = await response.Content
            .ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.NotNull(validationProblem);
        Assert.Contains("DifficultyLevel", validationProblem.Errors.Keys);
        Assert.Contains(
            "Difficulty level is invalid.",
            validationProblem.Errors["DifficultyLevel"]);
    }

    [Fact]
    public async Task UpdateExercise_ShouldReturn200WithUpdatedExercise()
    {
        await _factory.ResetDatabaseAsync();

        var request = new SaveExerciseRequest(
            Name: "Updated Squat",
            Description: "An updated lower-body exercise.",
            Instructions: ["Stand with the bar.", "Squat down.", "Stand back up."],
            MuscleGroup: "Legs",
            Equipment: "Barbell",
            DifficultyLevel: "Intermediate");

        var response = await _client.PutAsJsonAsync("/api/exercises/squat", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedExercise = await response.Content.ReadFromJsonAsync<ExerciseDto>();

        Assert.NotNull(updatedExercise);
        Assert.Equal("squat", updatedExercise.Id);
        Assert.Equal(request.Name, updatedExercise.Name);
        Assert.Equal(request.Description, updatedExercise.Description);

        var getResponse = await _client.GetAsync("/api/exercises/squat");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var persistedExercise =
            await getResponse.Content.ReadFromJsonAsync<ExerciseDto>();

        Assert.NotNull(persistedExercise);
        Assert.Equal(request.Name, persistedExercise.Name);
        Assert.Equal(request.Description, persistedExercise.Description);
        Assert.Equal(request.Instructions, persistedExercise.Instructions);
        Assert.Equal(request.MuscleGroup, persistedExercise.MuscleGroup);
        Assert.Equal(request.Equipment, persistedExercise.Equipment);
        Assert.Equal(request.DifficultyLevel, persistedExercise.DifficultyLevel);
    }
}
