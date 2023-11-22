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
        
            services.AddTransient<IGirafService, GirafService>();
            services.AddTransient<IAuthenticationService, GirafAuthenticationService>();
            services.AddTransient<IWeekService, WeekService>();
            return services;
        }
    }
}
