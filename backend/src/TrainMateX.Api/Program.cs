var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/exercises", () => Exercise.GetExercises());


app.Run();
