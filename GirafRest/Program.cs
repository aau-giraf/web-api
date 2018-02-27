using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GirafRest.Setup;
using Microsoft.AspNetCore.Hosting;
using MySql.Data.MySqlClient;

namespace GirafRest
{
    /// <summary>
    /// An enum to store the user's choice of database option.
    /// </summary>
    public enum DbOption { SQLite, MySQL }

    /// <summary>
    /// The main class for the Giraf REST-api.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Giraf REST Server.");

            //Parse all the program arguments and stop execution if any invalid arguments were found.
            var pa = new ProgramArgumentParser();
            bool validArguments = pa.CheckProgramArguments(args);
            if(!validArguments) return;

            //Build the host from the given arguments.
            try{
                var host = ConfigureHost();
                //Launch the rest-api.
                host.Run();
            }
            catch(MySqlException e){
                Console.WriteLine("Something went wrong in connecting to the MySql server: " +
                                  $"{e.Message}");              
            }
            catch(Exception e){
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Builds the host environment from a specified config class.
        /// <see cref="Startup"/> sets the general environment (authentication, logging i.e)
        /// <see cref="StartupLocal"/> sets up the environment for local development.
        /// <see cref="StartupDeployment"/> sets up the environment for deployment.
        /// </summary>
        /// <returns>A <see cref="IWebHost"/> host fit for running the server.</returns>
        private static IWebHost ConfigureHost() {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://+:{ProgramOptions.Port}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            return host;
        }
    }
}
