namespace TrainMateX.Api.Dtos;

public sealed record CreateWorkoutTemplateExerciseRequest(
    string? ExerciseId,
    int Sets,
    int Reps
);
