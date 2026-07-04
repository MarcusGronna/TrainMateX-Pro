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
    return await service.GetExercises();
});

app.MapGet("/api/exercises/{id}", async (string id, AppDbContext dbContext) =>
{
    var service = new ExerciseService(dbContext);
    return await service.GetExerciseById(id)
        is Exercise exercise
        ? Results.Ok(exercise)
        : Results.NotFound();
});

app.Run();

public partial class Program { }