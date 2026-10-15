using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.Application.IServices
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
    }
}
