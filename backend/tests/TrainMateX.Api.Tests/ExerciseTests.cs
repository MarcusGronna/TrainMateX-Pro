using Microsoft.EntityFrameworkCore;
using TrainMateX.Api.Dtos;
using TrainMateX.Api;

namespace TrainMateX.Api.Tests;

// TODO create AppDbContextFixture
public class ExerciseTests
{
    private readonly AppDbContext _context;

    public ExerciseTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _context.Exercises.AddRange(
            new Exercise
            {
                Id = "bench-press",
                Name = "Bench Press",
                Description = "Chest exercise",
                Instructions = ["Lie down", "Press bar"],
                MuscleGroup = "Chest",
                Equipment = "Barbell",
                DifficultyLevel = "Intermediate"
            },
            new Exercise
            {
                Id = "squat",
                Name = "Squat",
                Description = "Leg exercise",
                Instructions = ["Stand", "Squat down"],
                MuscleGroup = "Legs",
                Equipment = "Barbell",
                DifficultyLevel = "Intermediate"
            },
            new Exercise
            {
                Id = "deadlift",
                Name = "Deadlift",
                Description = "Back exercise",
                Instructions = ["Grip bar", "Lift"],
                MuscleGroup = "Back",
                Equipment = "Barbell",
                DifficultyLevel = "Advanced"
            });

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetExercises_ReturnsAllExercises()
    {
        var service = new ExerciseService(_context);
        var exercises = await service.GetExercisesAsync();

        Assert.Equal(_context.Exercises.Count(), exercises.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("non-existant-id")]
    [InlineData("BENCH-PRESS")]
    public async Task GetExerciseById_WithUnknownId_ReturnsNull(string id)
    {
        var service = new ExerciseService(_context);
        var exercise = await service.GetExerciseByIdAsync(id);

        Assert.Null(exercise);
    }

    [Fact]
    public async Task GetExerciseById_ReturnsExercise()
    {
        var service = new ExerciseService(_context);
        var exercise = await service.GetExerciseByIdAsync("bench-press");

        Assert.Equal("bench-press", exercise?.Id);
    }

    [Fact]
    public async Task CreateExercise_WithValidData_Created()
    {
        var exerciseRequest = new SaveExerciseRequest
        (
            Name: "Shoulder Press",
            Description: "Shoulder exercise",
            Instructions: ["Dumbbells by ear", "Push"],
            MuscleGroup: "Shoulders",
            Equipment: "Dumbbell",
            DifficultyLevel: "Intermediate"
        );

        var service = new ExerciseService(_context);
        var result = await service.CreateExerciseAsync(exerciseRequest);
        var exercise = await service.GetExerciseByIdAsync("shoulder-press");

        Assert.NotNull(exercise);
        Assert.True(result.Type == CreateExerciseResultType.Created);
        Assert.Equal("Shoulder Press", exercise.Name);
        Assert.Equal("shoulder-press", exercise.Id);
    }

    [Fact]
    public async Task CreateExercise_WithInvalidData_ShouldReturnError()
    {
        var exerciseRequest = new SaveExerciseRequest
        (
            Name: "",
            Description: "Shoulder exercise",
            Instructions: ["Dumbbells by ear", "Push"],
            MuscleGroup: "Shoulders",
            Equipment: "Dumbbell",
            DifficultyLevel: "Intermediate"
        );

        var countBefore = _context.Exercises.Count();
        var service = new ExerciseService(_context);
        var result = await service.CreateExerciseAsync(exerciseRequest);
        var exercise = await service.GetExerciseByIdAsync("shoulder-press");

        Assert.True(result.Type == CreateExerciseResultType.ValidationFailed);
        Assert.Contains(result.Errors, e => e.Key == "Name");
        Assert.Equal(countBefore, _context.Exercises.Count());
    }

    [Fact]
    public async Task CreateExercise_ShouldReturnErrors_WhenIdIsNull()
    {
        var exerciseRequest = new SaveExerciseRequest
        (
            Name: "!!!",
            Description: "Shoulder exercise",
            Instructions: ["Dumbbells by ear", "Push"],
            MuscleGroup: "Shoulders",
            Equipment: "Dumbbell",
            DifficultyLevel: "Intermediate"
        );

        var service = new ExerciseService(_context);
        var result = await service.CreateExerciseAsync(exerciseRequest);

        Assert.True(result.Type == CreateExerciseResultType.ValidationFailed);
        Assert.Contains(result.Errors, e => 
            e.Key == "Name" &&
            e.Value.SequenceEqual(["Name produces an invalid id."]));
    }
}
