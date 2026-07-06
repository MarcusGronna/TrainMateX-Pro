using Microsoft.EntityFrameworkCore;
using TrainMateX.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await ExerciseSeeder.SeedAsync(dbContext);
}

app.UseCors("AllowLocalhost3000");

app.MapGet("/api/exercises", async (AppDbContext dbContext) =>
{
    var service = new ExerciseService(dbContext);
    var exercises = await service.GetExercises();
    var response = exercises.Select(exercise => new ExerciseListDto(
        id: exercise.Id,
        name: exercise.Name,
        muscleGroup: exercise.MuscleGroup,
        difficultyLevel: exercise.DifficultyLevel));

    return Results.Ok(response);
});

app.MapGet("/api/exercises/{id}", async (string id, AppDbContext dbContext) =>
{
    var service = new ExerciseService(dbContext);
    var exercise = await service.GetExerciseById(id);

    if (exercise is null)
    {
        return Results.NotFound();
    }

    var response = new ExerciseDto(
        id: exercise.Id,
        name: exercise.Name,
        description: exercise.Description,
        instructions: exercise.Instructions,
        muscleGroup: exercise.MuscleGroup,
        equipment: exercise.Equipment,
        difficultyLevel: exercise.DifficultyLevel);

    return Results.Ok(response);
});

app.Run();

public partial class Program { }

public record ExerciseListDto(
    string id,
    string name,
    string muscleGroup,
    string difficultyLevel
);

public record ExerciseDto(
    string id,
    string name,
    string description,
    List<string> instructions,
    string muscleGroup,
    string equipment,
    string difficultyLevel
);
