using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.Domain.Entities
{
    public class VehicleCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string IconKey { get; set; }
        public decimal MinWeightKg { get; set; }
        public decimal? MaxWeightKg { get; set; }
        /// <summary>
        /// logic for Which weights belong in this category .
        /// </summary>
        /// <param name="weightKg"></param>
        /// <returns></returns>
        public bool Contains(decimal weightKg) =>
       weightKg >= MinWeightKg && (MaxWeightKg is null || weightKg < MaxWeightKg);
    }

}
