using System.Text.Json;

public class Exercise
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public List<string> Instructions { get; set; } = [];
    public string MuscleGroup { get; set; } = String.Empty;
    public string Equipment { get; set; } = String.Empty;
    public string DifficultyLevel { get; set; } = String.Empty;

    public static List<Exercise> GetExercises()
    {
        var json = File.ReadAllText("exercise-list-data.json");
        return JsonSerializer.Deserialize<List<Exercise>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }

    public static Exercise? GetExerciseById(string id)
    {
        var json = File.ReadAllText("exercise-details-data.json");
        var data = JsonSerializer.Deserialize<List<Exercise>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data?.FirstOrDefault(e => e.Id == id);
    }
}