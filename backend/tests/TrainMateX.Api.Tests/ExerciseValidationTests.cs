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

    [Fact]
    public void Validate_ShouldReturnDescriptionError_WhenDescriptionIsBlank()
    {
        var request = CreateValidRequest(Description: "");

        var result = ExerciseValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Description", result.Errors.Keys);
    }

    [Fact]
    public void Validate_ShouldRemoveBlankInstructionRows()
    {
        var request = CreateValidRequest(Instructions: [" Step one ", "", " ", " Step two"]);

        var result = ExerciseValidation.Validate(request);

        Assert.True(result.IsValid);
        Assert.Equal(["Step one", "Step two"], result.NormalizedInstructions);
    }

    [Fact]
    public void Validate_ShouldReturnInstructionsError_WhenNoInstructionsRemain()
    {
        var request = CreateValidRequest(
            Instructions: [" ", ""]
        );

        var result =
        ExerciseValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Instructions",
        result.Errors.Keys);
    }

    [Fact]
    public void Validate_ShouldReturnMuscleGroupError_WhenMuscleGroupIsInvalid()
    {
        var request = CreateValidRequest(MuscleGroup: "Neck");

        var result = ExerciseValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("MuscleGroup",
        result.Errors.Keys);
    }

    [Fact]
    public void Validate_ShouldReturnEquipmentError_WhenEquipmentIsInvalid()
    {
        var request = CreateValidRequest(Equipment: "Stone");

        var result = ExerciseValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("Equipment", result.Errors.Keys);
    }

    [Fact]
    public void Validate_ShouldReturnDifficultyLevelError_WhenDifficultyLevelIsInvalid()
    {
        var request = CreateValidRequest(DifficultyLevel: "Impossible");

        var result = ExerciseValidation.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains("DifficultyLevel",
        result.Errors.Keys);
    }

    private static SaveExerciseRequest CreateValidRequest(

        string Name = "Overhead Press",
        string Description = "A compound upper-body exercise",
        List<string>? Instructions = null,
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
