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

app.MapGet("/api/exercises", async (
    ExerciseService service,
    CancellationToken ct) =>
{
    var exercises = await service.GetExercisesAsync(ct);
    var response = exercises.Select(exercise => exercise.ToListDto());

    return Results.Ok(response);
});

app.MapGet("/api/exercises/{id}", async (
    string id,
    ExerciseService service,
    CancellationToken ct) =>
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

    return result.Type switch
    {
        CreateExerciseResultType.ValidationFailed =>
            Results.ValidationProblem(result.Errors),

        CreateExerciseResultType.Conflict =>
            Results.Conflict(result.Errors),

        CreateExerciseResultType.Created when result.Exercise is { } exercise =>
            Results.Created($"/api/exercises/{exercise.Id}", exercise.ToDto()),

        _ => Results.Problem()
    };
});

app.MapPut("/api/exercises/{id}", async (
    string id,
    SaveExerciseRequest request,
    ExerciseService service,
    CancellationToken ct) =>
{
    var result = await service.UpdateExerciseAsync(id, request, ct);

    return result.Type switch
    {
        UpdateExerciseResultType.NotFound =>
            Results.NotFound(),

        UpdateExerciseResultType.ValidationFailed =>
            Results.ValidationProblem(result.Errors),

        UpdateExerciseResultType.Updated when result.Exercise is { } exercise =>
            Results.Ok(exercise.ToDto()),

        _ => Results.Problem()
    };
});

app.MapDelete("/api/excercises/{id}", async (
    string id, 
    ExerciseService service, 
    CancellationToken ct) =>
{
    var deleted = await service.DeleteExerciseAsync(id, ct);

    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run();

public partial class Program { }
