using TrainMateX.Api.Dtos;
using TrainMateX.Api.Results;

namespace TrainMateX.Api;

public static class WorkoutTemplateValidation
{
    public static WorkoutTemplateValidationResult Validate(CreateWorkoutTemplateRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["Name"] = ["Name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors["Description"] = ["Description is required."];
        }

        if (request.Exercises is null || request.Exercises.Count == 0)
        {
            errors["Exercises"] = ["At least one exercise is required."];
        }
        
        else
        {
            for (var index = 0; index < request.Exercises.Count; index++)
            {
                var exercise = request.Exercises[index];

                if (string.IsNullOrWhiteSpace(exercise.ExerciseId))
                {
                    errors[$"Exercises[{index}].ExerciseId"] =
                        ["Exercise is required."];
                }

                if (exercise.Sets <= 0)
                {
                    errors[$"Exercises[{index}].Sets"] =
                        ["Sets must be greater than zero."];
                }

                if (exercise.Reps <= 0)
                {
                    errors[$"Exercises[{index}].Reps"] =
                        ["Reps must be greater than zero."];
                }
            }

            var hasDuplicateExerciseIds = request.Exercises
                  .Where(exercise => !string.IsNullOrWhiteSpace(exercise.ExerciseId))
                  .GroupBy(exercise => exercise.ExerciseId, StringComparer.Ordinal)
                  .Any(group => group.Count() > 1);

            if (hasDuplicateExerciseIds)
            {
                errors["Exercises"] =
                    ["An exercise may only appear once in a workout template."];
            }
        }

        return new WorkoutTemplateValidationResult(
            IsValid: errors.Count == 0,
            Errors: errors
        );
    }
}
