namespace TrainMateX.Api.Results;

public sealed record CreateExerciseResult
(
    CreateExerciseResultType Type,
    Exercise? Exercise,
    Dictionary<string, string[]> Errors
);