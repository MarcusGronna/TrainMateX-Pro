namespace TrainMateX.Api;

public sealed record CreateExerciseReslut
(
    CreateExerciseResultType Type,
    Exercise? Exercise,
    Dictionary<string, string[]> Errors
);