using GirafRest.IRepositories;
using GirafRest.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GirafRepositories
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMySql(configuration);
            // scoped, transient, singleton
            services.AddTransient<IActivityRepository, ActivityRepository>();
            return services;
        }
    }
}
