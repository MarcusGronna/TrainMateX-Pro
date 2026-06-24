namespace TrainMateX.Api.Tests;

public class ExerciseTests
{
    [Fact]
    public void GetExercises_ReturnsAllExercises()
    {
        var exercises = Exercise.GetExercises();

        Assert.Equal(3, exercises.Count);
    }

    // TODO: Implement test when Exercise has interface
    //[Fact]
    //public void GetExercises_WithNoDataSource_ReturnsNull()
    //{
    //    var exercises = Exercise.GetExercises();

    //    Assert.Null(exercises);
    //}

    [Theory]
    [InlineData("")]
    [InlineData("non-existant-id")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void GetExerciseById_WithUnknownId_ReturnsNull(string id)
    {
        var exercise = Exercise.GetExerciseById(id);

        Assert.Null(exercise);
    }

    [Fact]
    public void GetExerciseById_ReturnsExercise()
    {
        var exercise = Exercise.GetExerciseById("bench-press");

        Assert.Equal("bench-press", exercise?.Id);
    }
}
