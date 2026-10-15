using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Application.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string IconKey { get; set; }

        public decimal MinWeightKg { get; set; }

        public decimal? MaxWeightKg { get; set; } //Heavycategory has no upper limit.
    }
}
