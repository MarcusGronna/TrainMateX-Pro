namespace TrainMateX.Api;

public record ExerciseValidationResult
(
    bool IsValid,
    Dictionary<string, string[]> Errors,
    List<string> NormalizedInstructions
);
