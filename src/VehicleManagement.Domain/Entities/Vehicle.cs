using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Domain.Entities
{
    /// <summary>
    /// The Vehicle do not need a Category column. 
    /// The category is worked out from the weight each time,being always current.
    /// We don’t store the category on the vehicle here.
    /// </summary>
    public class Vehicle
    {
        public int Id { get; set; }
        public required string OwnerName { get; set; }
        public int ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        public int YearOfManufacture { get; set; }
        public decimal WeightKg { get; set; }
    }
}
