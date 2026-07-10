using Microsoft.EntityFrameworkCore;

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
}