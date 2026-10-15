using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Validations;

namespace VehicleManagement.UnitTests
{
    public class VehicleCategoryTests
    {
        private static VehicleCategory Cat(string name, decimal min, decimal? max) =>
            new() { Name = name, IconKey = "icon", MinWeightKg = min, MaxWeightKg = max };

        private static List<VehicleCategory> DefaultCategories() =>
            [Cat("Light", 0, 500), Cat("Medium", 500, 2500), Cat("Heavy", 2500, null)];

        // Asserting Single proves a weight resolves to exactly one category.
        private static string Resolve(decimal weightKg, IEnumerable<VehicleCategory> categories) =>
            Assert.Single(categories, c => c.Contains(weightKg)).Name;

        public static TheoryData<decimal, string> WeightsAndExpectedCategories => new()
    {
        { 0.01m, "Light" },
        { 499.99m, "Light" },
        { 500.00m, "Medium" },   // shared boundary belongs to the upper category
        { 2499.99m, "Medium" },
        { 2500.00m, "Heavy" },   // shared boundary belongs to the upper category
        { 1_000_000m, "Heavy" },
    };

        [Theory]
        [MemberData(nameof(WeightsAndExpectedCategories))]
        public void Weight_ResolvesToExactlyOneExpectedCategory(decimal weightKg, string expected)
        {
            Assert.Equal(expected, Resolve(weightKg, DefaultCategories()));
        }

        [Fact]
        public void ChangingCategoryBoundary_ReclassifiesExistingWeight()
        {
            var categories = DefaultCategories();
            Assert.Equal("Medium", Resolve(2200m, categories));

            // Medium now ends at 2000 kg and Heavy starts at 2000 kg.
            categories[1].MaxWeightKg = 2000m;
            categories[2].MinWeightKg = 2000m;

            Assert.Empty(CategoryRangeValidator.Validate(categories));
            Assert.Equal("Heavy", Resolve(2200m, categories));
        }
    }
    }
