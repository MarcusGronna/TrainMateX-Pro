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
            entity.HasKey(WorkoutTemplate => WorkoutTemplate.Id);

            entity.Property(WorkoutTemplate => WorkoutTemplate.Name).IsRequired();
            entity.Property(WorkoutTemplate => WorkoutTemplate.Description).IsRequired();
        });

        modelBuilder.Entity<WorkoutTemplateExercise>(entity =>
        {
            entity.HasKey(WorkoutTemplateExercise => new
            {
                WorkoutTemplateExercise.WorkoutTemplateId,
                WorkoutTemplateExercise.ExerciseId
            });

            entity.HasIndex(workoutTemplateExercise => new
            {
                workoutTemplateExercise.WorkoutTemplateId,
                workoutTemplateExercise.Position
            })
            .IsUnique();

            entity.Property(workoutTemplateExercise => workoutTemplateExercise.ExerciseId)
                .IsRequired();

            entity.HasOne(WorkoutTemplateExercise => WorkoutTemplateExercise.WorkoutTemplate)
                .WithMany(WorkoutTemplate => WorkoutTemplate.WorkoutTemplateExercises)
                .HasForeignKey(WorkoutTemplateExercise => WorkoutTemplateExercise.WorkoutTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(WorkoutTemplateExercise => WorkoutTemplateExercise.Exercise)
                .WithMany(exercise => exercise.WorkoutTemplateExercises)
                .HasForeignKey(WorkoutTemplateExercise => WorkoutTemplateExercise.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
