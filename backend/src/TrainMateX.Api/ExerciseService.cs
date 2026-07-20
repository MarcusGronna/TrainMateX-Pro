using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TrainMateX.Api.Dtos;

namespace TrainMateX.Api;

public class ExerciseService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<Exercise?> GetExerciseByIdAsync(string id)
    {
        return await _context.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Exercise>> GetExercisesAsync()
    {
        return await _context.Exercises.AsNoTracking().ToListAsync();
    }

    public async Task<CreateExerciseResult> CreateExerciseAsync(SaveExerciseRequest request)
    {
        var validatedResult = ExerciseValidation.Validate(request);

        if (!validatedResult.IsValid)
        {
            return new CreateExerciseResult
                (
                    Type: CreateExerciseResultType.ValidationFailed,
                    Exercise: null,
                    Errors: validatedResult.Errors
                );
        }

        var id = GenerateSlug(request.Name);

        if (string.IsNullOrWhiteSpace(id))
        {
            var errors = new Dictionary<string, string[]>(validatedResult.Errors)
            {
                ["Name"] = ["Name produces an invalid id."]
            };

            return new CreateExerciseResult
                (
                    Type: CreateExerciseResultType.ValidationFailed,
                    Exercise: null,
                    Errors: errors
                );
        }

        if (await ExerciseExistAsync(id))
        {
            var errors = new Dictionary<string, string[]>(validatedResult.Errors)
            {
                ["Name"] = ["An exercise with this name already exists."]
            };

            return new CreateExerciseResult
                (
                    Type: CreateExerciseResultType.Conflict,
                    Exercise: null,
                    Errors: errors
                );
        }

        var exercise = new Exercise
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Instructions = validatedResult.NormalizedInstructions,
            MuscleGroup = request.MuscleGroup,
            Equipment = request.Equipment,
            DifficultyLevel = request.DifficultyLevel
        };

        await _context.AddAsync(exercise);

        await _context.SaveChangesAsync();
        return new CreateExerciseResult
                (
                    Type: CreateExerciseResultType.Created,
                    Exercise: exercise,
                    Errors: validatedResult.Errors
                ); 
    }

    private async Task<bool> ExerciseExistAsync(string id)
    {
        return await _context.Exercises.AnyAsync(e => e.Id == id);
    }

    private static string GenerateSlug(string name)
    {
        var slug = Regex.Replace(
            name.Trim().ToLowerInvariant(), 
            @"[^a-z0-9]+", 
            "-"
        );

        return slug.Trim('-');
    }
}