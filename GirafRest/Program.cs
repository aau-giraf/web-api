using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Setup;
using Microsoft.AspNetCore.Hosting;

namespace GirafRest
{
    public enum DbOption { SQLite, MySQL }

    /// <summary>
    /// The main class for the Giraf REST-api.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The name of the connection string element in the XML configuration-file
        /// Defaults to the appsettings.Development.json file
        /// </summary>
        public static string ConnectionStringName = "appsettings.Development.json";

        /// <summary>
        /// A static field for storing the choice of database option.
        /// Defaults to SQLite
        /// </summary>
        public static DbOption DbOption = DbOption.SQLite;

        public static void Main(string[] args)
        {
            var helpMessage = "\tRun with --help to list options";
            var options = "\n\t--db=[mysql|sqlite] | Connect to MySQL or SQLite database, defaults to SQLite.\n" + 
                          "\t--prod=[true|false] | If true then connect to production db, defaults to false.\n" +
                          "\t--list              | List options\n";

            Console.Title = "Giraf REST-api Console";
            Console.WriteLine("Welcome to Giraf REST Server.");

            foreach (var arg in args) {
                switch (arg) {
                    case "--db=mysql":
                        DbOption = DbOption.MySQL;
                        break;
                    case "--db=sqlite":
                        DbOption = DbOption.SQLite;
                        break;
                    case "--prod=true":
                        ConnectionStringName = "appsettings.json";
                        break;
                    case "--prod=false":
                        ConnectionStringName = "appsettings.Development.json";
                        break;
                    case "--list":
                        Console.WriteLine(options);
                        return;
                    default:
                        Console.WriteLine("Invalid argument {0}", arg);
                        Console.WriteLine(helpMessage);
                        return;
                }
            }

            //Build the host from the given arguments.
            var host = ConfigureHost();

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
