namespace TrainMateX.Api.Results;

public sealed record WorkoutTemplateValidationResult(
    bool IsValid,
    Dictionary<string, string[]> Errors
);
