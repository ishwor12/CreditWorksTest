using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.Validation;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.UnitTests
{
    public class CategoryRequestValidatorTests
    {
        private static SaveCategoryRequest CreateValidRequest()
        {
            return new SaveCategoryRequest { Name = "Super Light", IconKey = "bicycle", MinWeightKg = 200 };
        }

        // The categories that already exist, not counting the one being created or edited.
        private static List<VehicleCategory> CreateExistingCategories()
        {
            return new List<VehicleCategory>
           {
               new VehicleCategory { Name = "Light", IconKey = "scooter", MinWeightKg = 0, MaxWeightKg = 500 },
               new VehicleCategory { Name = "Medium", IconKey = "car", MinWeightKg = 500, MaxWeightKg = 2500 },
               new VehicleCategory { Name = "Heavy", IconKey = "truck", MinWeightKg = 2500, MaxWeightKg = null }
           };
        }

        private static bool HasError(List<string> errors, string text)
        {
            return errors.Any(e => e.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void ValidRequest_HasNoErrors()
        {
            var errors = CategoryRequestValidator.Validate(CreateValidRequest(), CreateExistingCategories());

            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void EmptyName_IsRejected(string name)
        {
            var request = CreateValidRequest();
            request.Name = name;

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "category name is required"));
        }

        [Fact]
        public void NameLongerThan100Characters_IsRejected()
        {
            var request = CreateValidRequest();
            request.Name = new string('a', 101);

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "characters or fewer"));
        }

        [Fact]
        public void DuplicateName_IsRejected_IgnoringUpperAndLowerCase()
        {
            var request = CreateValidRequest();
            request.Name = "light";

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "already exists"));
        }

        [Fact]
        public void EmptyIcon_IsRejected()
        {
            var request = CreateValidRequest();
            request.IconKey = "";

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "category icon is required"));
        }

        [Fact]
        public void IconLongerThan50Characters_IsRejected()
        {
            var request = CreateValidRequest();
            request.IconKey = new string('a', 51);

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "characters or fewer"));
        }

        [Fact]
        public void NegativeMinimum_IsRejected()
        {
            var request = CreateValidRequest();
            request.MinWeightKg = -1;

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "cannot be negative"));
        }

        [Fact]
        public void MinimumWithMoreThanTwoDecimals_IsRejected()
        {
            var request = CreateValidRequest();
            request.MinWeightKg = 200.123m;

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "two decimal places"));
        }

        [Fact]
        public void MinimumUsedByAnotherCategory_IsRejected()
        {
            var request = CreateValidRequest();
            request.MinWeightKg = 500;

            var errors = CategoryRequestValidator.Validate(request, CreateExistingCategories());

            Assert.True(HasError(errors, "already starts at"));
        }

    }
}
