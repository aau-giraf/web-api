using System;
using Microsoft.Extensions.DependencyInjection;

namespace GirafServices
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
        
            // services.AddSingleton<ISERVICE, MERSERVICE>();
            return services;
        }
    }
}
