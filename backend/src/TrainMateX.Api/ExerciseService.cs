using Microsoft.EntityFrameworkCore;
using TrainMateX.Api;

public class ExerciseService(AppDbContext context)
{
    private AppDbContext _context = context;

    public async Task<Exercise?> GetExerciseById(string id)
    {
        return await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Exercise>> GetExercises()
    {
        return await _context.Exercises.ToListAsync();
    }
}