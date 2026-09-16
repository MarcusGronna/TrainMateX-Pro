namespace TrainMateX.Api.Dtos;

public sealed record CreateWorkoutTemplateRequest(
    string? Name,
    string? Description,
    List<CreateWorkoutTemplateExerciseRequest>? Exercises
);
