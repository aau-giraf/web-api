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
using Microsoft.AspNetCore.Identity;
using static GirafRest.Models.DTOs.GirafUserDTO;

namespace GirafRest.Extensions
{
    /// <summary>
    /// The class for extension-methods for Giraf REST-api.
    /// </summary>
    public static class GirafExtensions {
        /// <summary>
        /// Extension-method for configuring the application to use a local SQLite database.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        public static void AddSqlite(this IServiceCollection services) {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "giraf.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            services.AddDbContext<GirafSqliteDbContext>(options => options.UseSqlite(connection));
        }

        /// <summary>
        /// An extension-method for configuring the application to use a MySQL database.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        public static void AddMySql(this IServiceCollection services, IConfigurationRoot Configuration) {
            //Setup the connection to the sql server
            services.AddDbContext<GirafMySqlDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
        }

        /// <summary>
        /// An extension-method for setting up roles for use when authorizing users to methods.
        /// </summary>
        /// <param name="roleManager">A reference to the role manager for the application.</param>
        public static void EnsureRoleSetup(this RoleManager<GirafRole> roleManager)
        {
            if (roleManager.Roles.AnyAsync().Result)
                return;

            var Roles = new GirafRole[]
            {
                new GirafRole(GirafRole.Admin),
                new GirafRole(GirafRole.Guardian),
                new GirafRole(GirafRole.Citizen),
                new GirafRole(GirafRole.Department)
            };
            foreach (var role in Roles)
            {
                //A hacky way to run tasks synchronously
                var r = roleManager.CreateAsync(role).Result;
            }
        }

        /// <summary>
        /// Makes a list of roles, which the user is a part of.
        /// </summary>
        /// <param name="result">The list of roles, which the user is part of.</param>
        public static async Task<GirafRoles> makeRoleList(this RoleManager<GirafRole> roleManager, UserManager<GirafUser> userManager, GirafUser user)
        {
            GirafRoles userRole = new GirafRoles();
            foreach (var role in roleManager.Roles)
                if (await userManager.IsInRoleAsync(user, role.Name))
                    userRole = (GirafRoles)Enum.Parse(typeof(GirafRoles), role.Name);
            return userRole;
        }

        /// <summary>
        /// An extension-method for setting up policies for use when authorizing users to methods.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        public static void ConfigurePolicies(this IServiceCollection services)
        {
            // Create policies for method access using attribute [Authorize(Policy = "PolicyName")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy(GirafRole.RequireCitizen, policy => policy.RequireRole(GirafRole.Citizen));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(GirafRole.RequireGuardian, policy => policy.RequireRole(GirafRole.Guardian));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(GirafRole.RequireAdmin, policy => policy.RequireRole(GirafRole.Admin));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(GirafRole.RequireGuardianOrAdmin, policy => policy.RequireRole(GirafRole.Guardian, GirafRole.Admin));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(GirafRole.RequireDepartment, policy => policy.RequireRole(GirafRole.Department));
            });
        }

        /// <summary>
        /// Removes the default password requirements from ASP.NET and set them to a bare minimum.
        /// </summary>
        /// <param name="options">A reference to IdentityOptions, which is used to configure Identity.</param>
        public static void RemovePasswordRequirements(this IdentityOptions options) {
            //Set password requirements to an absolute bare minimum.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }

        /// <summary>
        /// Configures the server to override the default behaviour on unauthorized request. This method
        /// configures the server to simply return Unauthorized instead of redirecting to a login page.
        /// </summary>
        /// <param name="options">A reference to IdentityOptions, which is used to configure Identity.</param>
        public static void StopRedirectOnUnauthorized(this IdentityOptions options) {
            options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.FromResult(0);
                    },
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.FromResult(0);
                    }
                };
                options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
        }

        /// <summary>
        /// Configures logging for the server. Depending on the program arguments the server will either log
        /// solely to the console or both the console and a log-file (that may be found on host/logs/log-yyyyMMdd.txt).
        /// </summary>
        /// <param name="app">A reference to the application builder, that is used to define the behaviour of the server.</param>
        /// <param name="loggerFactory">A reference to the loggerFactory that is used to define the behaviour of the loggers.</param>
        public static void ConfigureLogging(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole()
                .AddDebug();
            if (ProgramOptions.LogToFile)
            {
                //Save log files corresponding to the strings defined in Program.cs, in this case logs/log.txt
                loggerFactory.AddFile(Path.Combine(ProgramOptions.LogDirectory, ProgramOptions.LogFilepath), LogLevel.Warning);
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

        public static string ToContentType(this PictogramImageFormat format)
        {
            switch (format)
            {
                case PictogramImageFormat.png:
                    return "image/png";
                case PictogramImageFormat.jpg:
                    return "image/jpeg";
                default:
                    return "";
            }
        }
    }
}