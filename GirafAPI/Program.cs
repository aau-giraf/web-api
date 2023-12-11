using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using GirafAPI.Setup;

namespace GirafAPI
{
    /// <summary>
    /// Base program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program configuration.
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Initialization method for running program
        /// </summary>
        /// <param name="args">program arguments</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Giraf REST Server. :-)");

            //Parse all the program arguments and stop execution if any invalid arguments were found.
            var pa = new ProgramArgumentParser();
            bool validArguments = pa.CheckProgramArguments(args);
            if (!validArguments) return;

            //Build the host from the given arguments.
            try
            {
                BuildWebHost(args).Run();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Something went wrong in connecting to the MySql server: " +
                                  $"{e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
        /// <summary>
        /// Builds the host environment from a specified config class.
        /// <see cref="Startup"/> sets the general environment (authentication, logging i.e)
        /// </summary>
        /// <returns>A <see cref="IWebHost"/> host fit for running the server.</returns>
        public static IWebHost BuildWebHost(string[] args)
        {
          return WebHost.CreateDefaultBuilder()
               .UseKestrel()
               .UseUrls($"http://+:{ProgramOptions.Port}")
               .UseIISIntegration()
               .UseStartup<Startup>()
               .ConfigureAppConfiguration((hostContext, config) =>
               {
                   config.Sources.Clear();
               })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
               .UseDefaultServiceProvider(options => options.ValidateScopes = false)
               .Build();
        }
       
    }
}
