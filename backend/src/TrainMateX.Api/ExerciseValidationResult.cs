namespace TrainMateX.Api;

public sealed record ExerciseValidationResult
(
    bool IsValid,
    Dictionary<string, string[]> Errors,
    List<string> NormalizedInstructions
);
