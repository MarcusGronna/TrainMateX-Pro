namespace TrainMateX.Api;

public class Exercise
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public List<string> Instructions { get; set; } = [];
    public string MuscleGroup { get; set; } = String.Empty;
    public string Equipment { get; set; } = String.Empty;
    public string DifficultyLevel { get; set; } = String.Empty;
}