using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.Abstractions;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.IServices;
using Microsoft.EntityFrameworkCore;

namespace VehicleManagement.Application.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IAppDbContext _dbcontext;

        public ManufacturerService(IAppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<ManufacturerDto>> GetAllAsync()
        {

            return await _dbcontext.Manufacturers
                .OrderBy(m => m.Name)
                .Select(m => new ManufacturerDto { 
                    Id = m.Id, Name = m.Name 
                })
                .ToListAsync();

        } 
    }
}
