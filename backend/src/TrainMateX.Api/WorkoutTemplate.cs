namespace TrainMateX.Api;

public class WorkoutTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<WorkoutTemplateExercise> WorkoutTemplateExercises { get; set; } = [];
}
