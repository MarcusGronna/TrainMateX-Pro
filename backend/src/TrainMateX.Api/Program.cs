var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/exercises", () => Exercise.GetExercises());

app.MapGet("/exercises/{id}", (string id) => 
    Exercise.GetExerciseById(id) is Exercise exercise 
    ? Results.Ok(exercise) 
    : Results.NotFound());

app.Run();
