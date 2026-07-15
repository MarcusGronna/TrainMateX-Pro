namespace TrainMateX.Api;

public sealed record CreateExerciseResult
(
    CreateExerciseResultType Type,
    Exercise? Exercise,
    Dictionary<string, string[]> Errors
);