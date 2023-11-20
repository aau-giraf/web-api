using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirafRest.IRepositories;
using GirafRest.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GirafServices
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
        
            services.AddSingleton<ISERVICE, MERSERVICE>();
            return services;
        }
    }
}
