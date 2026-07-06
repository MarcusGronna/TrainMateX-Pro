using Microsoft.EntityFrameworkCore;

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
                DifficultyLevel = "Medium"
            },
            new Exercise
            {
                Id = "squat",
                Name = "Squat",
                Description = "Leg exercise",
                Instructions = ["Stand", "Squat down"],
                MuscleGroup = "Legs",
                Equipment = "Barbell",
                DifficultyLevel = "Medium"
            },
            new Exercise
            {
                Id = "deadlift",
                Name = "Deadlift",
                Description = "Back exercise",
                Instructions = ["Grip bar", "Lift"],
                MuscleGroup = "Back",
                Equipment = "Barbell",
                DifficultyLevel = "Hard"
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
}
