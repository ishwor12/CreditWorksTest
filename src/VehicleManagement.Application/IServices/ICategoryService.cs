using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.Application.IServices
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(SaveCategoryRequest request);
        Task<CategoryDto> UpdateAsync(int id, SaveCategoryRequest request);
        Task DeleteAsync(int id);
    }
}
