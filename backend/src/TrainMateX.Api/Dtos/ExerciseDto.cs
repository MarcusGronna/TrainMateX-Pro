namespace TrainMateX.Api.Dtos;

public sealed record ExerciseDto(
    string Id,
    string Name,
    string Description,
    List<string> Instructions,
    string MuscleGroup,
    string Equipment,
    string DifficultyLevel
);
