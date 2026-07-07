namespace TrainMateX.Api.DTOs;

public record SaveExerciseRequest(
    string name,
    string description,
    List<string> instructions,
    string muscleGroup,
    string equipment,
    string difficultyLevel
);
    