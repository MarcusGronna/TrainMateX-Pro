namespace TrainMateX.Api.Tests;

public class ExerciseTests
{
    [Fact]
    public void GetExercises_ReturnsAllExercises()
    {
        var exercises = Exercise.GetExercises();

        Assert.Equal(3, exercises.Count);
    }

    [Fact]
    public void GetExerciseById_WithUnknownId_ReturnsNull()
    {
        var exercise = Exercise.GetExerciseById(String.Empty);

        Assert.Null(exercise);
    }
}
