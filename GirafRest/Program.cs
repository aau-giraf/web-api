using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Setup;
using Microsoft.AspNetCore.Hosting;

namespace GirafRest
{
    public enum DbOption { SQLite, SQL }

    /// <summary>
    /// The main class for the Giraf REST-api.
    /// </summary>
    public class Program
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
        public const string CONNECTIONSTRING_NAME = "ConnectionString";

        /// <summary>
        /// A static field for storing the choice of database option.
        /// </summary>
        public static DbOption DbOption;
        /// <summary>
        /// Program argument for running the server locally.
        /// </summary>
        private const string RUN_WITH_LOCALDB = "-localdb";
        /// <summary>
        /// Program argument for deploying the server.
        /// </summary>
        private const string RUN_DEPLOYMENT = "-deploy";

        public static void Main(string[] args)
        {
            Console.Title = "Giraf REST-api Console";
            Console.WriteLine("Welcome to Giraf REST Server.");

            //Default to use SQLite database
            DbOption = DbOption.SQLite;

            //Display a message for the user in case no arguments were specified.
            if(args.Length == 0) {
                System.Console.WriteLine("You have not specified any arguments, please use the following:");
                System.Console.WriteLine($"   {RUN_WITH_LOCALDB}       : Runs a local SQLite db for debugging (default)");
                System.Console.WriteLine($"   {RUN_DEPLOYMENT} #path# : Deploys the server #path# must be the path to a web.config file containing a ConnectionString for the MariaDB.");
                DbOption = DbOption.SQLite;
            }

            //Build the host from the given arguments.
            IWebHost host = ConfigureHost();
            //Check if server should deploy and verify that there is two arguments, validation of the path is handled later.
            if(args.Length > 0 && args[0].Equals(RUN_DEPLOYMENT)) {
                if(string.IsNullOrEmpty(args[1])) 
                    throw new ArgumentException("You must specify a file-path as second argument when running with " + RUN_DEPLOYMENT);

                ConfigurationFilePath = args[1];
                DbOption = DbOption.SQL;
            }
            //Launch the rest-api.
            host.Run();
        }

        /// <summary>
        /// Builds the host environment from a specified config class.
        /// <see cref="Startup"/> sets the general environment (authentication, logging i.e)
        /// <see cref="StartupLocal"/> sets up the environment for local development.
        /// <see cref="StartupDeployment"/> sets up the environment for deployment.
        /// </summary>
        /// <returns>A <see cref="IWebHost"/> host fit for running the server.</returns>
        private static IWebHost ConfigureHost(string port = "5000") {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://localhost:{port}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            return host;
        }
    }
}
