using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.Validation;
using Xunit;

namespace VehicleManagement.UnitTests;

public class CategoryInputValidatorTests
{
    private static List<CategoryInputDto> CreateValidInputs()
    {
        return new List<CategoryInputDto>
           {
               new CategoryInputDto { Id = 1, Name = "Light", IconKey = "scooter", MinWeightKg = 0, MaxWeightKg = 500 },
               new CategoryInputDto { Id = 2, Name = "Medium", IconKey = "car", MinWeightKg = 500, MaxWeightKg = 2500 },
               new CategoryInputDto { Id = 3, Name = "Heavy", IconKey = "truck", MinWeightKg = 2500, MaxWeightKg = null }
           };
    }

    private static bool HasError(List<string> errors, string text)
    {
        return errors.Any(e => e.Contains(text, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidInputs_HaveNoErrors()
    {
        var errors = CategoryInputValidator.Validate(CreateValidInputs());

        Assert.Empty(errors);
    }

    [Fact]
    public void EmptyList_IsRejected()
    {
        var errors = CategoryInputValidator.Validate(new List<CategoryInputDto>());

        Assert.True(HasError(errors, "at least one category"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyName_IsRejected(string name)
    {
        var inputs = CreateValidInputs();
        inputs[1].Name = name;

        var errors = CategoryInputValidator.Validate(inputs);

        Assert.True(HasError(errors, "Row 2: category name is required"));
    }

    [Fact]
    public void NameLongerThan100Characters_IsRejected()
    {
        var inputs = CreateValidInputs();
        inputs[1].Name = new string('a', 101);

        var errors = CategoryInputValidator.Validate(inputs);

        Assert.True(HasError(errors, "characters or fewer"));
    }

    [Fact]
    public void DuplicateName_IsRejected_IgnoringUpperAndLowerCase()
    {
        var inputs = CreateValidInputs();
        inputs[1].Name = "light";

        var errors = CategoryInputValidator.Validate(inputs);

        Assert.True(HasError(errors, "used more than once"));
    }

    [Fact]
    public void EmptyIcon_IsRejected()
    {
        var inputs = CreateValidInputs();
        inputs[2].IconKey = "";

        var errors = CategoryInputValidator.Validate(inputs);

        Assert.True(HasError(errors, "Row 3: category icon is required"));
    }

    [Fact]
    public void IconLongerThan50Characters_IsRejected()
    {
        var inputs = CreateValidInputs();
        inputs[0].IconKey = new string('a', 51);

        var errors = CategoryInputValidator.Validate(inputs);

        Assert.True(HasError(errors, "characters or fewer"));
    }

    [Fact]
    public void SameCategoryTwice_IsRejected()
    {
        var inputs = CreateValidInputs();
        inputs[1].Id = 1;

        var errors = CategoryInputValidator.Validate(inputs);

        Assert.True(HasError(errors, "appears more than once"));
    }
}