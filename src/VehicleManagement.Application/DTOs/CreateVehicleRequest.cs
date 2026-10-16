using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Application.DTOs
{
    public class CreateVehicleRequest
    {
        public string OwnerName { get; set; } = string.Empty;

        public int ManufacturerId { get; set; }

        public int YearOfManufacture { get; set; }

        public decimal WeightKg { get; set; }
    }
}
