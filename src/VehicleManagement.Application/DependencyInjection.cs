using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.IServices;
using VehicleManagement.Application.Services;

namespace VehicleManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<ICategoryService, CategoryService>();
            return services;
        }
    }
}
