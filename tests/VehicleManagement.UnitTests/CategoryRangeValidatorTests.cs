using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Validations;

namespace VehicleManagement.UnitTests
{
    public class CategoryRangeValidatorTests
    {
        private static VehicleCategory Cat(string name, decimal min, decimal? max) =>
       new() { Name = name, IconKey = "icon", MinWeightKg = min, MaxWeightKg = max };

        private static List<VehicleCategory> DefaultCategories() =>
            [Cat("Light", 0, 500), Cat("Medium", 500, 2500), Cat("Heavy", 2500, null)];

        [Fact]
        public void DefaultCategories_AreValid()
        {
            Assert.Empty(CategoryRangeValidator.Validate(DefaultCategories()));
        }

        [Fact]
        public void Gap_IsRejected()
        {
            // 500-600 kg is not covered.
            List<VehicleCategory> categories = [Cat("Light", 0, 500), Cat("Medium", 600, 2500), Cat("Heavy", 2500, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("gap", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Overlap_IsRejected()
        {
            // 550 kg would belong to both Light and Medium.
            List<VehicleCategory> categories = [Cat("Light", 0, 600), Cat("Medium", 500, 2500), Cat("Heavy", 2500, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("overlap", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void DuplicateMinimum_IsRejectedAsOverlap()
        {
            List<VehicleCategory> categories = [Cat("Light", 0, 500), Cat("Other", 0, 2500), Cat("Heavy", 2500, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("overlap", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void LowestCategory_MustStartAtZero()
        {
            List<VehicleCategory> categories = [Cat("Light", 100, 500), Cat("Heavy", 500, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("start at 0", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void HighestCategory_MustHaveNoUpperLimit()
        {
            List<VehicleCategory> categories = [Cat("Light", 0, 500), Cat("Medium", 500, 2500), Cat("Heavy", 2500, 5000)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("highest category", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void OpenEndedCategory_MustBeLast()
        {
            List<VehicleCategory> categories = [Cat("Light", 0, null), Cat("Medium", 500, 2500), Cat("Heavy", 2500, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("no upper limit", StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData(500, 500)]
        [InlineData(500, 400)]
        public void Maximum_MustBeGreaterThanMinimum(int min, int max)
        {
            List<VehicleCategory> categories = [Cat("Light", 0, 500), Cat("Broken", min, max), Cat("Heavy", 500, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("greater than the minimum", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Boundaries_WithMoreThanTwoDecimals_AreRejected()
        {
            List<VehicleCategory> categories = [Cat("Light", 0, 500.005m), Cat("Heavy", 500.005m, null)];

            var errors = CategoryRangeValidator.Validate(categories);

            Assert.Contains(errors, e => e.Contains("two decimal places", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void EmptySet_IsRejected()
        {
            Assert.NotEmpty(CategoryRangeValidator.Validate(new List<VehicleCategory>()));
        
        }
    }
}
