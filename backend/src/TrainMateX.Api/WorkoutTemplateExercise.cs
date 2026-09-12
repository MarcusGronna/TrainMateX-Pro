namespace TrainMateX.Api;

public class WorkoutTemplateExercise
{
    public Guid WorkoutTemplateId { get; set; }
    public string ExerciseId { get; set; } = string.Empty;

    public int Position { get; set; }
    public int Set { get; set; }
    public int Reps { get; set; }

    public WorkoutTemplate WorkoutTemplate { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
}