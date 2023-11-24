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
            services.AddTransient<IAuthenticationService, GirafAuthenticationService>();
            
            // User
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            
            // Weekplanner
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IWeekBaseService, WeekBaseService>();
            services.AddTransient<IWeekService, WeekService>();
            services.AddTransient<IWeekTemplateService, WeekTemplateService>();
            
            // top level
            services.AddTransient<ISettingService, SettingService>();


            
            return services;
        }
    }
}
