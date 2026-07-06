using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace TrainMateX.Api;

public class ExerciseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.Exercises.AnyAsync())
        {
            return;
        }

        var json = await File.ReadAllTextAsync("exercise-details-data.json");

        var exercises = JsonSerializer.Deserialize<List<Exercise>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];

        dbContext.Exercises.AddRange(exercises);
        await dbContext.SaveChangesAsync();
    }
}
