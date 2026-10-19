using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.Application.IServices
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();

        // Saves the whole category list in 1 time: updates, new rows and deleted rows.
        Task<List<CategoryDto>> SaveAllAsync(List<CategoryInputDto> inputs);
    }
}
