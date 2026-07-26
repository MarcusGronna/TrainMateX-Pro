namespace TrainMateX.Api.Results;

public sealed record UpdateExerciseResult
(
    UpdateExerciseResultType Type,
    Exercise? Exercise,
    Dictionary<string, string[]> Errors
);
