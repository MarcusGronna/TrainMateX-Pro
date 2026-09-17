namespace TrainMateX.Api.Results;

public sealed record CreateWorkoutTemplateResult(
    CreateWorkoutTemplateResultType Type,
    WorkoutTemplate? WorkoutTemplate,
    Dictionary<string, string[]> Errors
);

