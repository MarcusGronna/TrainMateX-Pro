using TrainMateX.Api.Dtos;

namespace TrainMateX.Api.Mappers;

public static class ExerciseMapper
{
    public static ExerciseDto ToDto(this Exercise exercise) =>
        new(
            Id: exercise.Id,
            Name: exercise.Name,
            Description: exercise.Description,
            Instructions: exercise.Instructions,
            MuscleGroup: exercise.MuscleGroup,
            Equipment: exercise.Equipment,
            DifficultyLevel: exercise.DifficultyLevel);


    public static ExerciseListDto ToListDto(this Exercise exercise) =>
        new(
            Id: exercise.Id,
            Name: exercise.Name,
            MuscleGroup: exercise.MuscleGroup,
            DifficultyLevel: exercise.DifficultyLevel);
}
