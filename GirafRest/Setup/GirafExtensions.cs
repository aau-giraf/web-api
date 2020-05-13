using System;
using System.IO;
using System.Threading.Tasks;
using GirafRest.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace GirafRest.Extensions
{
    /// <summary>
    /// The class for extension-methods for Giraf REST-api.
    /// </summary>
    public static class GirafExtensions {
        /// <summary>
        /// An extension-method for configuring the application to use a MySQL database.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        /// <param name="Configuration">Contains the ConnectionString</param>
        public static void AddMySql(this IServiceCollection services, IConfigurationRoot Configuration) {
            //Setup the connection to the sql server
            services.AddDbContext<GirafDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
        }

        /// <summary>
        /// An extension-method for setting up roles for use when authorizing users to methods.
        /// </summary>
        /// <param name="roleManager">A reference to the role manager for the application.</param>
        public static void EnsureRoleSetup(this RoleManager<GirafRole> roleManager)
        {
            if (roleManager == null) {
                throw new System.ArgumentNullException(roleManager + " is null");
            }
            if (roleManager.Roles.AnyAsync().Result)
                return;

            var Roles = new GirafRole[]
            {
                new GirafRole(GirafRole.SuperUser),
                new GirafRole(GirafRole.Guardian),
                new GirafRole(GirafRole.Citizen),
                new GirafRole(GirafRole.Department)
            };
            foreach (var role in Roles)
            {
                //A hacky way to run tasks synchronously, do not use this if at all avoidable
                var r = roleManager.CreateAsync(role).Result;
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
            if (userManager == null) {
                throw new System.ArgumentNullException(userManager + " is null");
            }
            GirafRoles userRole = new GirafRoles();
            var roles = await userManager.GetRolesAsync(user).ConfigureAwait(true);
            if(roles.Count != 0) 
                userRole = (GirafRoles)Enum.Parse(typeof(GirafRoles), roles[0]);
            return userRole;
        }

        /// <summary>
        /// Removes the default password requirements from ASP.NET and set them to a bare minimum.
        /// </summary>
        /// <param name="options">A reference to IdentityOptions, which is used to configure Identity.</param>
        public static void RemovePasswordRequirements(this IdentityOptions options) {
            if (options == null) {
                throw new System.ArgumentNullException(options + " is null");
            }
            //Set password requirements to an absolute bare minimum.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }

        /// <summary>
        /// Configures logging for the server. Depending on the program arguments the server will either log
        /// solely to the console or both the console and a log-file (that may be found on host/logs/log-yyyyMMdd.txt).
        /// </summary>
        /// <param name="app">A reference to the application builder, that is used to define the behaviour of the server.</param>
        public static void ConfigureLogging(this IApplicationBuilder app)
        {
            if (ProgramOptions.LogToFile)
            {
                app.UseStaticFiles();

                //Ensure that the folder for the log-files exists - create it if not.
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), ProgramOptions.LogDirectory);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                //Tells ASP.NET that the log-directory is accessible remotely on the /logs-url
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(
                        directoryPath),
                    RequestPath = new PathString("/logs")
                });
            }
        }
    }
}