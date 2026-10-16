using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Application.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }

        public required string OwnerName { get; set; }

        public required string ManufacturerName { get; set; }

        public int YearOfManufacture { get; set; }

        public decimal WeightKg { get; set; }

        public required string CategoryName { get; set; }

        public required string CategoryIconKey { get; set; }
    }
}
