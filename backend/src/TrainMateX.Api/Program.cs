using Microsoft.EntityFrameworkCore;
using TrainMateX.Api;
using TrainMateX.Api.Dtos;
using TrainMateX.Api.Mappers;

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

app.MapGet("/api/exercises", async (ExerciseService service, CancellationToken ct) =>
{
    var exercises = await service.GetExercisesAsync(ct);
    var response = exercises.Select(exercise => exercise.ToListDto());

    return Results.Ok(response);
});

app.MapGet("/api/exercises/{id}", async (string id, ExerciseService service, CancellationToken ct) =>
{
    var exercise = await service.GetExerciseByIdAsync(id, ct);

    if (exercise is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(exercise.ToDto());
});

app.MapPost("/api/exercises", async (
    SaveExerciseRequest request, 
    ExerciseService service, 
    CancellationToken ct) =>
{
    var result = await service.CreateExerciseAsync(request, ct);

    if (result.Type == CreateExerciseResultType.ValidationFailed)
    {
        return Results.ValidationProblem(result.Errors);
    }

    if (result.Type == CreateExerciseResultType.Conflict)
    {
        return Results.Conflict(result.Errors);
    }

    if (result.Type == CreateExerciseResultType.Created && result.Exercise is not null)
    {
        var response = result.Exercise.ToDto();

        return Results.Created($"/api/exercises/{response.Id}", response);
    }

    return Results.Problem();
});

app.Run();

public partial class Program { }
