using TrainMateX.Api.Dtos;

namespace TrainMateX.Api.Tests;

public class WorkoutTemplateValidationTests
{
    [Fact]
    public void Validate_WithValidRequest_ReturnsValid()
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithBlankName_ReturnsNameError()
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(name: " "));

        Assert.False(result.IsValid);
        Assert.Contains("Name", result.Errors.Keys);
        Assert.Equal(["Name is required."], result.Errors["Name"]);
    }

    [Fact]
    public void Validate_WithBlankDescription_ReturnsDescriptionError()
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(description: ""));

        Assert.False(result.IsValid);
        Assert.Contains("Description", result.Errors.Keys);
    }

    [Fact]
    public void Validate_WithNullExercises_ReturnsExercisesError()
    {
        var request = CreateValidRequest() with { Exercises = null };

        var result = WorkoutTemplateValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Exercises", result.Errors.Keys);
    }

    [Fact]
    public void Validate_WithEmptyExercises_ReturnsExercisesError()
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(exercises: []));

        Assert.False(result.IsValid);
        Assert.Contains("Exercises", result.Errors.Keys);
    }

    [Fact]
    public void Validate_WithBlankExerciseId_ReturnsIndexedExerciseIdError()
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(
            exercises:
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: " ",
                    Sets: 4,
                    Reps: 8)
            ]));

        Assert.False(result.IsValid);
        Assert.Contains("Exercises[0].ExerciseId", result.Errors.Keys);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveSets_ReturnsIndexedSetsError(int sets)
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(
            exercises:
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "bench-press",
                    Sets: sets,
                    Reps: 8)
            ]));

        Assert.False(result.IsValid);
        Assert.Contains("Exercises[0].Sets", result.Errors.Keys);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveReps_ReturnsIndexedRepsError(int reps)
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(
            exercises:
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "bench-press",
                    Sets: 4,
                    Reps: reps)
            ]));

        Assert.False(result.IsValid);
        Assert.Contains("Exercises[0].Reps", result.Errors.Keys);
    }

    [Fact]
    public void Validate_WithDuplicateExerciseIds_ReturnsExercisesError()
    {
        var result = WorkoutTemplateValidation.Validate(CreateValidRequest(
            exercises:
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "bench-press",
                    Sets: 4,
                    Reps: 8),
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "bench-press",
                    Sets: 3,
                    Reps: 10)
            ]));

        Assert.False(result.IsValid);
        Assert.Contains("Exercises", result.Errors.Keys);
    }

    private static CreateWorkoutTemplateRequest CreateValidRequest(
        string? name = "Push Day",
        string? description = "Chest, shoulder and triceps training.",
        List<CreateWorkoutTemplateExerciseRequest>? exercises = null)
    {
        return new CreateWorkoutTemplateRequest(
            Name: name,
            Description: description,
            Exercises: exercises ??
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "bench-press",
                    Sets: 4,
                    Reps: 8)
            ]);
    }
}
