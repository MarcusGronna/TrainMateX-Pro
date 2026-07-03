using Microsoft.EntityFrameworkCore;

namespace TrainMateX.Api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Exercise> Exercises => Set<Exercise>();  
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.MuscleGroup).IsRequired();
            entity.Property(e => e.Equipment).IsRequired();
            entity.Property(e => e.DifficultyLevel).IsRequired();

            entity.Property(e => e.Instructions).HasColumnType("jsonb");
        });
    }
}
