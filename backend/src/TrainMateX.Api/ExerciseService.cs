using Microsoft.EntityFrameworkCore;
using TrainMateX.Api.Dtos;

namespace TrainMateX.Api;

public class ExerciseService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<Exercise?> GetExerciseById(string id)
    {
        return await _context.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Exercise>> GetExercises()
    {
        return await _context.Exercises.AsNoTracking().ToListAsync();
    }

    public async Task<ExerciseValidationResult> CreateExercise(SaveExerciseRequest request)
    {
        var validatedResult = ExerciseValidation.Validate(request);

        if (!validatedResult.IsValid)
        {
            return validatedResult;
        }

        var id = GenerateSlug(request.Name);

        if (string.IsNullOrWhiteSpace(id))
        {
            var errors = new Dictionary<string, string[]>(validatedResult.Errors)
            {
                ["Name"] = ["Name produces an invalid id."]
            };

            return validatedResult with
            {
                IsValid = false,
                Errors = errors
            };
        }

        if (await ExerciseAlreadyExistAsync(id))
        {
            var errors = new Dictionary<string, string[]>(validatedResult.Errors)
            {
                ["Name"] = ["An exercise with this name already exists."]
            };

            return validatedResult with
            {
                IsValid = false,
                Errors = errors
            };
        }

        await _context.AddAsync(new Exercise
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Instructions = validatedResult.NormalizedInstructions,
            MuscleGroup = request.MuscleGroup,
            Equipment = request.Equipment,
            DifficultyLevel = request.DifficultyLevel
        });

        await _context.SaveChangesAsync();
        return validatedResult;
    }

    private async Task<bool> ExerciseAlreadyExistAsync(string id)
    {
        return await _context.Exercises.AnyAsync(e => e.Id == id);
    }

    private string GenerateSlug(string name)
    {
        return name.Trim().ToLower().Replace(' ', '-');
    }
}