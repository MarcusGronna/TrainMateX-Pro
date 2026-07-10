using TrainMateX.Api.Dtos;

namespace TrainMateX.Api.Tests;

public class ExerciseValidationTests
{
    [Fact]
    public void Validate_ShouldReturnValid_WhenRequestIsValid()
    {
        var request = CreateValidRequest();

        var result = ExerciseValidation.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldReturnNameError_WhenNameIsBlank()
    {
        var request = CreateValidRequest(Name: " ");

        var result = ExerciseValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Name", result.Errors.Keys);
    }

    private static SaveExerciseRequest CreateValidRequest(

        string Name = "Overhead Press",
        string Description = "A compound upper-body exercise",
        List<string> Instructions = null,
        string MuscleGroup = "Shoulders",
        string Equipment = "Barbell",
        string DifficultyLevel = "Intermediate")
    {
        return new SaveExerciseRequest(
            Name: Name,
            Description: Description,
            Instructions: Instructions ?? ["Press the bar overhead."],
            MuscleGroup: MuscleGroup,
            Equipment: Equipment,
            DifficultyLevel: DifficultyLevel

        );
    }

}
