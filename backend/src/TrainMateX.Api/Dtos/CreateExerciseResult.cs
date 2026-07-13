namespace TrainMateX.Api.Dtos;

public sealed record CreateExerciseResult
(
    CreateExerciseResultType Type,
    Exercise? Exercise,
    Dictionary<string, string[]> Errors
);