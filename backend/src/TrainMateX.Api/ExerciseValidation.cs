using System.Collections.Frozen;
using TrainMateX.Api.Dtos;

namespace TrainMateX.Api;

public static class ExerciseValidation
{
    private static readonly FrozenSet<string> AllowedMuscleGroups =
        new[]
        {
            "Chest",
            "Back",
            "Legs",
            "Shoulders",
            "Arms",
            "Core"
        }.ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> AllowedEquipment =
        new[]
        {
            "Barbell",
            "Dumbbell",
            "Bodyweight",
            "Machine",
            "Cable",
            "Kettlebell"
        }.ToFrozenSet(StringComparer.Ordinal);

    private static readonly FrozenSet<string> AllowedDifficultyLevels =
        new[]
        {
            "Beginner",
            "Intermediate",
            "Advanced"
        }.ToFrozenSet(StringComparer.Ordinal);

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
            .ToList() ?? [];
    }
}