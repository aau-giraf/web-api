using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using GirafRest.Data;
using GirafRest.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.Extensions;
using Pomelo.EntityFrameworkCore.MySql;

namespace GirafRest.Extensions
{
    /// <summary>
    /// The class for extension-methods for Giraf REST-api.
    /// </summary>
    public static class GirafExtensions {
        /// <summary>
        /// Extension-method for configuring the application to use a local Sqlite database.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        public static void AddSqlite(this IServiceCollection services) {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "GirafDB.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            services.AddDbContext<GirafDbContext>(options => options.UseSqlite(connection));
        }

        /// <summary>
        /// An extension-method for configuring the application to use a Sql database. This method is used when deploying the server.
        /// A path to a valid xml-file containing the connection-string must be given as the second argument when running the application.
        /// Example: dotnet run -deploy ~/web-api/connection.xml
        /// This xml-file MUST contain a key called ConnectionString.
        /// </summary>
        /// <param name="services">A reference to the services of the application.</param>
        public static void AddSql(this IServiceCollection services, IConfigurationRoot Configuration) {
            //Setup the connection to the sql server
            services.AddDbContext<GirafDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
        }

        public static void RemovePasswordRequirements(this IdentityOptions options) {
            //Set password requirements to an absolute bare minimum.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }

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
    }
}