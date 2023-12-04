using System;
using GirafServices.Authentication;
using GirafServices.User;
using GirafServices.WeekPlanner;
using Microsoft.Extensions.DependencyInjection;

namespace GirafServices
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            
            // Authentication 
            services.AddScoped<IAuthenticationService, GirafAuthenticationService>();
            
            // User
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            
            // Weekplanner
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IWeekBaseService, WeekBaseService>();
            services.AddScoped<IWeekService, WeekService>();
            services.AddScoped<IWeekTemplateService, WeekTemplateService>();
            services.AddScoped<IPictogramService, PictogramService>();
            
            // top level
            services.AddScoped<ISettingService, SettingService>();


            
            return services;
        }
    }
}
