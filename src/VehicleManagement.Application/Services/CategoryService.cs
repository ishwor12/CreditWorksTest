using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.Abstractions;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.IServices;
using Microsoft.EntityFrameworkCore;

namespace VehicleManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IAppDbContext _dbcontext;

        public CategoryService(IAppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _dbcontext.VehicleCategories
                .OrderBy(c => c.MinWeightKg)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconKey = c.IconKey,
                    MinWeightKg = c.MinWeightKg,
                    MaxWeightKg = c.MaxWeightKg
                })
                .ToListAsync();
        }
    }
}
