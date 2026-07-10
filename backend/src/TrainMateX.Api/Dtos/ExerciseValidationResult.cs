namespace TrainMateX.Api.Dtos;

public sealed record ExerciseValidationResult
(
    bool IsValid,
    Dictionary<string, string[]> Errors,
    List<string> NormalizedInstructions
);
