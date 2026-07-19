using TrainMateX.Api.Dtos;

namespace TrainMateX.Api;

public static class ExerciseValidation
{
    public static readonly string[] AllowedMuscleGroups =
    [
        "Chest",
        "Back",
        "Legs",
        "Shoulders",
        "Arms",
        "Core"
    ];

    public static readonly string[] AllowedEquipment =
    [
        "Barbell",
        "Dumbbell",
        "Bodyweight",
        "Machine",
        "Cable",
        "Kettlebell"
    ];

    public static readonly string[] AllowedDifficultyLevels =
    [
        "Beginner",
        "Intermediate",
        "Advanced"
    ];

    public static ExerciseValidationResult Validate(SaveExerciseRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        var normalizedInstructions = NormalizeInstructions(request.Instructions);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["Name"] = ["Name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors["Description"] = ["Description is required."];
        }

        if (normalizedInstructions.Count == 0)
        {
            errors["Instructions"] = ["At least one instruction is required."];
        }

        if (!AllowedMuscleGroups.Contains(request.MuscleGroup))
        {
            errors["MuscleGroup"] = ["Muscle group is invalid."];
        }

        if (!AllowedEquipment.Contains(request.Equipment))
        {
            errors["Equipment"] = ["Equipment is invalid."];
        }

        if (!AllowedDifficultyLevels.Contains(request.DifficultyLevel))
        {
            errors["DifficultyLevel"] = ["Difficulty level is invalid."];
        }

        return new ExerciseValidationResult(
            IsValid: errors.Count == 0,
            Errors: errors,
            NormalizedInstructions: normalizedInstructions
        );
    }

    public static List<string> NormalizeInstructions(List<string>? instructions)
    {
        return instructions?
            .Where(instruction => !string.IsNullOrWhiteSpace(instruction))
            .Select(instruction => instruction.Trim())
            .Where(instruction => !string.IsNullOrWhiteSpace(instruction))
            .ToList() ?? [];
    }
}