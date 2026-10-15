using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.Application.IServices
{
    public interface IManufacturerService
    {
        Task<List<ManufacturerDto>> GetAllAsync();
    }
}
