using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/exercises", () => Exercise.GetExercises());


app.Run();


public class Exercise
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string MuscleGroup { get; set; } = String.Empty;
    public string DifficultyLevel { get; set; } = String.Empty;

    public static List<Exercise> GetExercises()
    {
        var json = File.ReadAllText("exercise-list-data.json");
        var data = JsonSerializer.Deserialize<Dictionary<string, Exercise>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data?.Values.ToList() ?? [];
    }
}