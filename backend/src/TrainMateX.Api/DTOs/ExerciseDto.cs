namespace TrainMateX.Api.DTOs;

public record ExerciseDto(
    string id,
    string name,
    string description,
    List<string> instructions,
    string muscleGroup,
    string equipment,
    string difficultyLevel
);
