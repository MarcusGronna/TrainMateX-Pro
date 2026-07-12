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

        if (validatedResult.IsValid)
        {
            await _context.AddAsync(new Exercise
            {
                Id = GenerateSlug(request.Name),
                Name = request.Name,
                Description = request.Description,
                Instructions = request.Instructions,
                MuscleGroup = request.MuscleGroup,
                Equipment = request.Equipment,
                DifficultyLevel = request.DifficultyLevel
            });

            await _context.SaveChangesAsync();
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