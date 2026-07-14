using Microsoft.EntityFrameworkCore;
using TrainMateX.Api;
using TrainMateX.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExerciseService>();

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

app.MapGet("/api/exercises", async (ExerciseService service) =>
{
    var exercises = await service.GetExercisesAsync();
    var response = exercises.Select(exercise => new ExerciseListDto(
        Id: exercise.Id,
        Name: exercise.Name,
        MuscleGroup: exercise.MuscleGroup,
        DifficultyLevel: exercise.DifficultyLevel));

    return Results.Ok(response);
});

app.MapGet("/api/exercises/{id}", async (string id, ExerciseService service) =>
{
    var exercise = await service.GetExerciseByIdAsync(id);

    if (exercise is null)
    {
        return Results.NotFound();
    }

    var response = new ExerciseDto(
        Id: exercise.Id,
        Name: exercise.Name,
        Description: exercise.Description,
        Instructions: exercise.Instructions,
        MuscleGroup: exercise.MuscleGroup,
        Equipment: exercise.Equipment,
        DifficultyLevel: exercise.DifficultyLevel);

    return Results.Ok(response);
});

app.Run();

public partial class Program { }
