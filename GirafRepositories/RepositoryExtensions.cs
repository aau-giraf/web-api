using GirafEntities.User;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRepositories.WeekPlanner;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        
        /// <summary>
        /// An extension-method for configuring the application to use a MySQL database.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        /// <param name="Configuration">Contains the ConnectionString</param>
        private static void AddMySql(this IServiceCollection services, IConfiguration Configuration)
        {
            //Setup the connection to the sql server
            services.AddDbContext<GirafDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version())));
        }
        
        /// <summary>
        /// An extension-method for setting up roles for use when authorizing users to methods.
        /// </summary>
        /// <param name="roleManager">A reference to the role manager for the application.</param>
        public async static Task EnsureRoleSetup(this RoleManager<GirafRole> roleManager)
        {
            bool allRolesExists = true;
            foreach (string role in Enum.GetNames(typeof(GirafRoles)))
            {
                GirafRole girafRole = await roleManager.FindByNameAsync(role);

                if (girafRole == null)
                    allRolesExists = false;
            }

            if (allRolesExists)
            {
                var Roles = new GirafRole[]
                {
                    new GirafRole(GirafRole.SuperUser),
                    new GirafRole(GirafRole.Guardian),
                    new GirafRole(GirafRole.Citizen),
                    new GirafRole(GirafRole.Department),
                    new GirafRole(GirafRole.Trustee)
                };
                foreach (var role in Roles)
                    await roleManager.CreateAsync(role);
                return;
            }
        }
        
        /// <summary>
        /// Creates a list of roles a given user is part of
        /// </summary>
        /// <param name="roleManager">Reference to the roleManager</param>
        /// <param name="userManager">Reference to the userManager</param>
        /// <param name="user">The user in question</param>
        /// <returns>
        /// Instance of GirafRole enum
        /// </returns>
        public static async Task<GirafRoles> findUserRole(
            this RoleManager<GirafRole> roleManager,
            UserManager<GirafUser> userManager,
            GirafUser user)
        {
            GirafRoles userRole = new GirafRoles();
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Count != 0)
                userRole = (GirafRoles)Enum.Parse(typeof(GirafRoles), roles[0]);
            return userRole;
        }
        
    }
}
