using Microsoft.EntityFrameworkCore;

namespace TrainMateX.Api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutTemplate> WorkoutTemplates => Set<WorkoutTemplate>();
    public DbSet<WorkoutTemplateExercise> WorkoutTemplateExercises => Set<WorkoutTemplateExercise>();

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

            entity.Property(e => e.Instructions)
                .HasColumnType("jsonb");
        });

        modelBuilder.Entity<WorkoutTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();

            entity.HasMany(workoutTemplate => workoutTemplate.WorkoutTemplateExercises)
                .WithOne(workoutTemplateExercise => workoutTemplateExercise.WorkoutTemplate)
                .HasForeignKey(workoutTemplateExercise => workoutTemplateExercise.WorkoutTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkoutTemplateExercise>(entity =>
        {
            entity.HasKey(e => new
            {
                e.WorkoutTemplateId,
                e.ExerciseId
            });

            entity.Property(e => e.WorkoutTemplateId).IsRequired();
            entity.Property(e => e.ExerciseId).IsRequired();
            entity.Property(e => e.Position).IsRequired();
            entity.Property(e => e.Set).IsRequired();
            entity.Property(e => e.Reps).IsRequired();

            entity.HasOne(e => e.WorkoutTemplate)
                .WithMany(e => e.WorkoutTemplateExercises)
                .HasForeignKey(e => e.WorkoutTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Exercise)
                .WithMany()
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
