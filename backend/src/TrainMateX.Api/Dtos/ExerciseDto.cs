namespace TrainMateX.Api.Dtos;

public record ExerciseDto(
    string Id,
    string Name,
    string Description,
    List<string> Instructions,
    string MuscleGroup,
    string Equipment,
    string DifficultyLevel
);
