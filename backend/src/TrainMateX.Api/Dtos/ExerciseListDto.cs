namespace TrainMateX.Api.Dtos;

public sealed record ExerciseListDto(
    string Id,
    string Name,
    string MuscleGroup,
    string DifficultyLevel
);
