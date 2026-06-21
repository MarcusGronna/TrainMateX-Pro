using System.Text.Json;

public class Exercise
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string MuscleGroup { get; set; } = String.Empty;
    public string DifficultyLevel { get; set; } = String.Empty;

    public static List<Exercise> GetExercises()
    {
        var json = File.ReadAllText("exercise-list-data.json");
        return JsonSerializer.Deserialize<List<Exercise>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }
}