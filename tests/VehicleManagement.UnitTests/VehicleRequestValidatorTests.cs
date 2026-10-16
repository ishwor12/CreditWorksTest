using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.Validation;

namespace VehicleManagement.UnitTests
{
    public class VehicleRequestValidatorTests
    {
        // A fixed year for test.
        private const int CurrentYear = 2026;

        private static CreateVehicleRequest CreateValidRequest()
        {
            return new CreateVehicleRequest
            {
                OwnerName = "John Smith",
                ManufacturerId = 1,
                YearOfManufacture = 2019,
                WeightKg = 450
            };
        }

        private static bool HasError(List<string> errors, string text)
        {
            return errors.Any(e => e.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void ValidRequest_HasNoErrors()
        {
            var errors = VehicleRequestValidator.Validate(CreateValidRequest(), CurrentYear);

            Assert.Empty(errors);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void EmptyOwnerName_IsRejected(string ownerName)
        {
            var request = CreateValidRequest();
            request.OwnerName = ownerName;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "owner's name is required"));
        }

        [Fact]
        public void OwnerNameLongerThan200Characters_IsRejected()
        {
            var request = CreateValidRequest();
            request.OwnerName = new string('a', 201);

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "characters or fewer"));
        }

        [Fact]
        public void ManufacturerNotSelected_IsRejected()
        {
            var request = CreateValidRequest();
            request.ManufacturerId = 0;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "manufacturer is required"));
        }

        [Theory]
        [InlineData(1885)]
        [InlineData(2028)]
        public void YearOutsideAllowedRange_IsRejected(int year)
        {
            var request = CreateValidRequest();
            request.YearOfManufacture = year;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "year of manufacture must be between"));
        }

        [Theory]
        [InlineData(1886)]
        [InlineData(2027)]
        public void YearAtTheEdgeOfTheAllowedRange_IsAccepted(int year)
        {
            var request = CreateValidRequest();
            request.YearOfManufacture = year;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void WeightZeroOrNegative_IsRejected(int weight)
        {
            var request = CreateValidRequest();
            request.WeightKg = weight;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "greater than 0"));
        }

        [Fact]
        public void WeightWithMoreThanTwoDecimals_IsRejected()
        {
            var request = CreateValidRequest();
            request.WeightKg = 10.123m;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "two decimal places"));
        }

        [Fact]
        public void WeightWithTwoDecimals_IsAccepted()
        {
            var request = CreateValidRequest();
            request.WeightKg = 1850.75m;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.Empty(errors);
        }

        [Fact]
        public void WeightAboveTheMaximum_IsRejected()
        {
            var request = CreateValidRequest();
            request.WeightKg = 1000001m;

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.True(HasError(errors, "must not exceed"));
        }

        [Fact]
        public void AllFieldsInvalid_ReturnsOneErrorPerField()
        {
            var request = new CreateVehicleRequest
            {
                OwnerName = "",
                ManufacturerId = 0,
                YearOfManufacture = 1800,
                WeightKg = -5
            };

            var errors = VehicleRequestValidator.Validate(request, CurrentYear);

            Assert.Equal(4, errors.Count);
        }

    }
}
