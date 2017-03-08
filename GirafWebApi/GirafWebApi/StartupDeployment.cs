using System.IO;
using System.Xml.Linq;
using GirafWebApi.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;

namespace GirafWebApi
{
    public class StartupDeployment : Startup
    {
        /// <summary>
        /// The absolute filepath for the configuration-file on the server.
        /// Must contain the key "ConnectionString" with the connection-string for the database.
        /// </summary>
        /// <returns></returns>
        public static string ConfigurationFilePath { get; set; }
        /// <summary>
        /// The name of the connection string element in the XML configuration-file
        /// </summary>
        private const string CONNECTIONSTRING_NAME = "ConnectionString";

        /// <summary>
        /// Creates a new instance of the class. This method is called by the Framework when the application starts.
        /// </summary>
        /// <param name="env"></param>
        public StartupDeployment(IHostingEnvironment env) : base(env){ }

        /// <summary>
        /// Configure the services of the api for deploytment. This sets up a connection to the SQL database.
        /// In order for this to work the program must be started with -deployment #path#, where #path# is the
        /// absolute path to an XML-file with the ConnectionString.
        /// </summary>
        /// <param name="services"></param>
        public override void ConfigureServices(IServiceCollection services) {
            base.ConfigureServices(services);
            //Open the XML document on the specified path and check that the file actually exists
            XDocument config = XDocument.Load(new System.Uri(ConfigurationFilePath).AbsoluteUri);
            if(config == null) {
                throw new FileNotFoundException("Failed to find a suitable XML-based configuration file");
            }
            //Extract the connection string
            var connString = config.Element(CONNECTIONSTRING_NAME);
            if(connString == null) {
                throw new ArgumentNullException($"The XML file on specified path must contain an element called {CONNECTIONSTRING_NAME}.\nThe given path was: {ConfigurationFilePath}");
            }

            //Setup the connection to the sql server
            services.AddDbContext<GirafDbContext>(options => options.UseSqlServer(connString.Value));
        }
    }
}