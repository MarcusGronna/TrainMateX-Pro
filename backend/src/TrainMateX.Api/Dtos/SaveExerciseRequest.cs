namespace TrainMateX.Api.Dtos;

public record SaveExerciseRequest(
    string Name,
    string Description,
    List<string> Instructions,
    string MuscleGroup,
    string Equipment,
    string DifficultyLevel
);
    