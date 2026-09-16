namespace TrainMateX.Api.Dtos;

public sealed record SaveExerciseRequest(
    string Name,
    string Description,
    List<string> Instructions,
    string MuscleGroup,
    string Equipment,
    string DifficultyLevel
);
    