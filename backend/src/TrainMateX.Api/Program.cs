var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/exercises", () => Exercise.GetExercises());

app.MapGet("/api/exercises/{id}", (string id) => 
    Exercise.GetExerciseById(id) is Exercise exercise 
    ? Results.Ok(exercise) 
    : Results.NotFound());

app.Run();

public partial class Program { }