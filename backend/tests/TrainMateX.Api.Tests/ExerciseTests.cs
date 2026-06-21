namespace TrainMateX.Api.Tests;

public class ExerciseTests
{
    [Fact]
    public void GetExercises_ReturnsAllExercises()
    {
        var exercises = Exercise.GetExercises();

        Assert.Equal(3, exercises.Count);
    }
}
