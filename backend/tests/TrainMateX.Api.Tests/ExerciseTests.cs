using Microsoft.EntityFrameworkCore;
using TrainMateX.Api.Dtos;

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
        var exercises = await service.GetExercises();

        Assert.Equal(3, exercises.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("non-existant-id")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task GetExerciseById_WithUnknownId_ReturnsNull(string id)
    {
        var service = new ExerciseService(_context);
        var exercise = await service.GetExerciseById(id);

        Assert.Null(exercise);
    }

    [Fact]
    public async Task GetExerciseById_ReturnsExercise()
    {
        var service = new ExerciseService(_context);
        var exercise = await service.GetExerciseById("bench-press");

        Assert.Equal("bench-press", exercise?.Id);
    }

    [Fact]
    public async Task CreateExercise_WithValidData_ReturnsIsValidValidationResult()
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
        var result = await service.CreateExercise(exerciseRequest);
        var exercise = await service.GetExerciseById("shoulder-press");


        Assert.NotNull(exercise);
        Assert.True(result.IsValid);
        Assert.Equal("Shoulder Press", exercise.Name);
        Assert.Equal("shoulder-press", exercise.Id);
    }
}
