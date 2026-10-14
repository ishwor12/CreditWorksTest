using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Domain.Entities
{
    /// <summary>
    ///  Reference data for vehicle manufacturers.
    ///Kept in the database so the list can change without a release.
    /// </summary>
    public class Manufacturer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
