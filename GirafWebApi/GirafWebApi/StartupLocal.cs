using GirafWebApi.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace GirafWebApi
{
    public class StartupLocal : Startup
    {
        /// <summary>
        /// Creates a new instance of the class. The class is automatically instantiated by the ASP.net framework.
        /// </summary>
        /// <param name="env">A reference to the <see cref="IHostingEnvironment"/>, which is handled by DependencyInjection</param>
        public StartupLocal(IHostingEnvironment env) : base(env){ }

        /// <summary>
        /// Configure services for local deployment. This involves setting up a local SQLite server, which is saved instance
        /// bin/
        /// </summary>
        /// <param name="services">A collection of the Services that must run on the server.</param>
        public override void ConfigureServices(IServiceCollection services) {
            base.ConfigureServices(services);
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "Giraf_SQLite.db" };
        	var connectionString = connectionStringBuilder.ToString();
        	var connection = new SqliteConnection(connectionString);

            services.AddDbContext<GirafDbContext>(options => options.UseSqlite(connection));
        }
    }
}