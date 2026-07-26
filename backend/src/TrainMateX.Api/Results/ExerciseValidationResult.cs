namespace TrainMateX.Api.Results;

public sealed record ExerciseValidationResult
(
    bool IsValid,
    Dictionary<string, string[]> Errors,
    List<string> NormalizedInstructions
);
