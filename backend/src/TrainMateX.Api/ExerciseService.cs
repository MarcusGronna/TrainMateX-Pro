using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TrainMateX.Api.Dtos;
using TrainMateX.Api.Results;

namespace TrainMateX.Api;

public class ExerciseService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<Exercise?> GetExerciseByIdAsync(
        string id,
        CancellationToken ct = default)
    {
        return await _context.Exercises
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<List<Exercise>> GetExercisesAsync(CancellationToken ct = default)
    {
        return await _context.Exercises
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<CreateExerciseResult> CreateExerciseAsync(
        SaveExerciseRequest request,
        CancellationToken ct = default)
    {
        var validationResult = ExerciseValidation.Validate(request);

        if (!validationResult.IsValid)
        {
            return new CreateExerciseResult(
                    Type: CreateExerciseResultType.ValidationFailed,
                    Exercise: null,
                    Errors: validationResult.Errors);
        }

        var id = GenerateSlug(request.Name);

        if (string.IsNullOrWhiteSpace(id))
        {
            var errors = new Dictionary<string, string[]>(validationResult.Errors)
            {
                ["Name"] = ["Name produces an invalid id."]
            };

            return new CreateExerciseResult(
                    Type: CreateExerciseResultType.ValidationFailed,
                    Exercise: null,
                    Errors: errors);
        }

        if (await ExerciseExistsAsync(id, ct))
        {
            var errors = new Dictionary<string, string[]>(validationResult.Errors)
            {
                ["Name"] = ["An exercise with this name already exists."]
            };

            return new CreateExerciseResult(
                    Type: CreateExerciseResultType.Conflict,
                    Exercise: null,
                    Errors: errors);
        }

        var exercise = new Exercise
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Instructions = validationResult.NormalizedInstructions,
            MuscleGroup = request.MuscleGroup,
            Equipment = request.Equipment,
            DifficultyLevel = request.DifficultyLevel
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync(ct);

        return new CreateExerciseResult(
                    Type: CreateExerciseResultType.Created,
                    Exercise: exercise,
                    Errors: validationResult.Errors);
    }

    public async Task<UpdateExerciseResult> UpdateExerciseAsync(
        string id,
        SaveExerciseRequest request,
        CancellationToken ct = default)
    {
        var validationResult = ExerciseValidation.Validate(request);

        if (!validationResult.IsValid)
        {
            return new UpdateExerciseResult(
                    Type: UpdateExerciseResultType.ValidationFailed,
                    Exercise: null,
                    Errors: validationResult.Errors);
        }

        var exercise = await _context.Exercises
            .FirstOrDefaultAsync(exercise => exercise.Id == id, ct);

        if (exercise is null)
        {
            return new UpdateExerciseResult(
                    Type: UpdateExerciseResultType.NotFound,
                    Exercise: null,
                    Errors: validationResult.Errors);
        }

        exercise.Name = request.Name;
        exercise.Description = request.Description;
        exercise.Instructions = validationResult.NormalizedInstructions;
        exercise.MuscleGroup = request.MuscleGroup;
        exercise.Equipment = request.Equipment;
        exercise.DifficultyLevel = request.DifficultyLevel;

        await _context.SaveChangesAsync(ct);

        return new UpdateExerciseResult(
                    Type: UpdateExerciseResultType.Updated,
                    Exercise: exercise,
                    Errors: validationResult.Errors);
    }

    private async Task<bool> ExerciseExistsAsync(
        string id,
        CancellationToken ct)
    {
        return await _context.Exercises
            .AnyAsync(e => e.Id == id, ct);
    }

    private static string GenerateSlug(string name)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        var slug = Regex.Replace(
            normalizedName,
            @"[^a-z0-9]+",
            "-");

        return slug.Trim('-');
    }
}
