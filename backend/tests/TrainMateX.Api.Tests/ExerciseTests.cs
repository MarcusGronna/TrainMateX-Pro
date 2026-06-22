namespace TrainMateX.Api.Tests;

public class ExerciseTests
{
    [Fact]
    public void GetExercises_ReturnsAllExercises()
    {
        var exercises = Exercise.GetExercises();

        Assert.Equal(3, exercises.Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("non-existant-id")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void GetExerciseById_WithUnknownId_ReturnsNull(string id)
    {
        var exercise = Exercise.GetExerciseById(id);

        Assert.Null(exercise);
    }
}
