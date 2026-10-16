using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Application.DTOs
{
    public class SaveCategoryRequest
    {
        public string Name { get; set; } = string.Empty;

        public string IconKey { get; set; } = string.Empty;

        // Inclusive start of the range. The end is calculated by next category.
        public decimal MinWeightKg { get; set; }

    }
}
