using Microsoft.EntityFrameworkCore;
using TrainMateX.Api.Dtos;

namespace TrainMateX.Api.Tests;

public class WorkoutTemplateServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly WorkoutTemplateService _service;

    public WorkoutTemplateServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _context.Exercises.AddRange(
            CreateExercise("bench-press", "Bench Press"),
            CreateExercise("squat", "Squat"),
            CreateExercise("deadlift", "Deadlift")
        );

        _context.SaveChanges();

        _service = new WorkoutTemplateService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private static Exercise CreateExercise(string id, string name)
    {
        return new Exercise
        {
            Id = id,
            Name = name,
            Description = $"{name} description",
            Instructions = ["Perform the exercise."],
            MuscleGroup = "Chest",
            Equipment = "Barbell",
            DifficultyLevel = "Intermediate"
        };
    }

    private static CreateWorkoutTemplateRequest CreateValidRequest(
        string name = "Full Body")
    {
        return new CreateWorkoutTemplateRequest(
            Name: name,
            Description: "A complete training session.",
            Exercises:
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "squat",
                    Sets: 4,
                    Reps: 6
                ),
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "bench-press",
                    Sets: 3,
                    Reps: 10
                )
            ]);
    }

    [Fact]
    public async Task CreateWorkoutTemplateAsync_WithValidRequest_PersistsAggregate()
    {
        var result = await _service.CreateWorkoutTemplateAsync(
            CreateValidRequest()
        );

        Assert.Equal(
            CreateWorkoutTemplateResultType.Created,
            result.Type
        );
        Assert.NotNull(result.WorkoutTemplate);
        Assert.NotEqual(Guid.Empty, result.WorkoutTemplate.Id);
        Assert.Empty(result.Errors);

        _context.ChangeTracker.Clear();

        var persistedTemplate = await _context.WorkoutTemplates
            .Include(template => template.WorkoutTemplateExercises)
            .SingleAsync();

        Assert.Equal("Full Body", persistedTemplate.Name);
        Assert.Equal(2, persistedTemplate.WorkoutTemplateExercises.Count);
    }

    [Fact]
    public async Task CreateWorkoutTemplateAsync_AssignsContiguousPositionsInRequestOrder()
    {
        var result = await _service.CreateWorkoutTemplateAsync(
            CreateValidRequest()
        );

        Assert.Equal(
            CreateWorkoutTemplateResultType.Created,
            result.Type
        );

        _context.ChangeTracker.Clear();

        var persistedTemplate = await _context.WorkoutTemplates
            .Include(template => template.WorkoutTemplateExercises)
            .SingleAsync();

        var rows = persistedTemplate.WorkoutTemplateExercises
            .OrderBy(row => row.Position)
            .ToList();

        Assert.Equal("squat", rows[0].ExerciseId);
        Assert.Equal(1, rows[0].Position);
        Assert.Equal(4, rows[0].Sets);
        Assert.Equal(6, rows[0].Reps);

        Assert.Equal("bench-press", rows[1].ExerciseId);
        Assert.Equal(2, rows[1].Position);
        Assert.Equal(3, rows[1].Sets);
        Assert.Equal(10, rows[1].Reps);
    }

    [Fact]
    public async Task CreateWorkoutTemplateAsync_WithInvalidRequest_DoesNotPersist()
    {
        var result = await _service.CreateWorkoutTemplateAsync(
            CreateValidRequest(name: " ")
        );

        Assert.Equal(
            CreateWorkoutTemplateResultType.ValidationFailed,
            result.Type
        );
        Assert.Null(result.WorkoutTemplate);
        Assert.Contains("Name", result.Errors);

        Assert.Equal(0, await _context.WorkoutTemplates.CountAsync());
        Assert.Equal(0, await _context.WorkoutTemplateExercises.CountAsync());
    }

    [Fact]
    public async Task CreateWorkoutTemplateAsync_WithUnknownExercise_ReturnsValidationFailureAndPersistsNothing()
    {
        var request = new CreateWorkoutTemplateRequest(
            Name: "Mixed Exercises",
            Description: "Includes an unknown exercise.",
            Exercises:
            [
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "squat",
                    Sets: 4,
                    Reps: 6
                ),
                new CreateWorkoutTemplateExerciseRequest(
                    ExerciseId: "missing-exercise",
                    Sets: 3,
                    Reps: 10
                )
            ]);

        var result = await _service.CreateWorkoutTemplateAsync(request);

        Assert.Equal(
            CreateWorkoutTemplateResultType.ValidationFailed,
            result.Type
        );
        Assert.Null(result.WorkoutTemplate);

        Assert.Contains(
            "Exercises[1].ExerciseId",
            result.Errors.Keys
        );

        Assert.Empty(await _context.WorkoutTemplates.ToListAsync());
        Assert.Empty(await _context.WorkoutTemplateExercises.ToListAsync());
    }

    [Fact]
    public async Task CreateWorkoutTemplateAsync_WithDuplicateName_CreatatesSeparateTemplates()
    {
        var request = CreateValidRequest();

        var firstResult = await _service.CreateWorkoutTemplateAsync(request);
        var secondResult = await _service.CreateWorkoutTemplateAsync(request);

        Assert.Equal(CreateWorkoutTemplateResultType.Created, firstResult.Type);
        Assert.Equal(CreateWorkoutTemplateResultType.Created, secondResult.Type);

        Assert.NotNull(firstResult.WorkoutTemplate);
        Assert.NotNull(secondResult.WorkoutTemplate);
        Assert.NotEqual(Guid.Empty, firstResult.WorkoutTemplate.Id);
        Assert.NotEqual(Guid.Empty, secondResult.WorkoutTemplate.Id);
        Assert.NotEqual(
            firstResult.WorkoutTemplate.Id,
            secondResult.WorkoutTemplate.Id
        );

        Assert.Equal(2, await _context.WorkoutTemplates.CountAsync());
    }
}
