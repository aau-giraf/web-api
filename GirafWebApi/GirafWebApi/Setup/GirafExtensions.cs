using System;
using System.IO;
using System.Xml.Linq;
using GirafWebApi.Setup;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GirafWebApi.Extensions
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
        public static void AddSql(this IServiceCollection services) {
            //Open the XML document on the specified path and check that the file actually exists
            XDocument config = XDocument.Load(new System.Uri(Program.ConfigurationFilePath).AbsoluteUri);
            if(config == null) {
                throw new FileNotFoundException("Failed to find a suitable XML-based configuration file");
            }
            //Extract the connection string
            var connString = config.Element(Program.CONNECTIONSTRING_NAME);
            if(connString == null) {
                throw new ArgumentNullException($"The XML file on specified path must contain an element called {Program.CONNECTIONSTRING_NAME}.\nThe given path was: {Program.ConfigurationFilePath}");
            }
            //Setup the connection to the sql server
            services.AddDbContext<GirafDbContext>(options => options.UseSqlServer(connString.ToString()));
        }
    }
}