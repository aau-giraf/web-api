using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static bool GenerateSampleData = false;

        /// <summary>
        /// A static field for storing the choice of database option.
        /// Defaults to SQLite
        /// </summary>
        public static DbOption DbOption = DbOption.SQLite;

        /// <summary>
        /// A field for storing the port on which to host the server.
        /// </summary>
        private static Int16 port = 5000;

        public static bool LogToFile = false;
        public static string LogFilepath = "";
        public static readonly string LogDirectory = Path.Combine("wwwroot", "Logs");

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Giraf REST Server.");

            //Parse all the program arguments and stop execution if any invalid arguments were found.
            bool validArguments = checkProgramArguments(args);
            if(!validArguments) return;

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
        private static IWebHost ConfigureHost() {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://+:{port}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            return host;
        }

        private static bool checkProgramArguments(string[] args) {
            Regex portRegex = new Regex(@"port=[\d]*");

            var helpMessage = "\tRun with --help to list options";
            var options = "\n\t--db=[mysql|sqlite]\t\t| Connect to MySQL or SQLite database, defaults to SQLite.\n" +
                          "\t--prod=[true|false]\t\t| If true then connect to production db, defaults to false.\n" +
                          "\t--port=integer\t\t\t| Specify which port to host the server on, defaults to 5000.\n" +
                          "\t--list\t\t\t\t| List options\n" +
                          "\t--sample-data=[true|false]\t| Defines if the rest-api should generate some sample data. This only works on an empty database." +
                          "\t--logfile=string\t\t| Toggles logging to a file, the string specifies the path to the file relative to the working directory.";
            if(args.Length == 0) {
                System.Console.WriteLine("\tNo program arguments were found - running in default configuration.");
                System.Console.WriteLine(options);
            }

            foreach (var arg in args) {
                string[] argumentAndParameter = arg.Split('=');

                switch (argumentAndParameter[0]) {
                    //Check for db arguments and validate them
                    case "--db":
                        if(argumentAndParameter[1].Equals("mysql"))
                            DbOption = DbOption.MySQL;
                        else if(argumentAndParameter[1].Equals("sqlite"))
                            DbOption = DbOption.SQLite;
                        else {
                            System.Console.WriteLine("\tERROR: Invalid database parameter was specified. Expected [sqlite|mysql] but found " + argumentAndParameter[1]);
                            return false;
                        }
                        break;
                    //Check for production arguments and validate them
                    case "--prod":
                        if(argumentAndParameter[1].Equals("true"))
                            ConnectionStringName = "appsettings.json";
                        else if(argumentAndParameter[1].Equals("false"))
                            ConnectionStringName = "appsettings.Development.json";
                        else {
                            System.Console.WriteLine("\tERROR: Invalid production parameter was specified, expected [true|false] but found " + argumentAndParameter[1]);
                            return false;
                        }
                        break;
                    //Check for list arguments and display a list of program arguments.
                    case "--list":
                        Console.WriteLine(options);
                        return false;
                    //Check for port arguments and validate them
                    case "--port":
                        try {
                            port = Int16.Parse(argumentAndParameter[1]);
                        }
                        catch {
                            System.Console.WriteLine("\tERROR: Invalid port parameter was specified, expected an integer, but found " + argumentAndParameter[1]);
                            return false;
                        }
                        break;
                    case "--sample-data":
                        try
                        {
                            GenerateSampleData = bool.Parse(argumentAndParameter[1]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\tERROR: Invalid parameter specified for --sample-data, expected [true|false], but found " + argumentAndParameter[1]);
                            return false;
                        }
                        break;
                    case "--logfile":
                        if (String.IsNullOrWhiteSpace(argumentAndParameter[1]))
                            return false;
                        LogToFile = true;
                        LogFilepath = argumentAndParameter[1];
                        break;
                    //An invalid argument was found, stop the execution.
                    default:
                        Console.WriteLine("\tInvalid argument {0}", argumentAndParameter[0]);
                        Console.WriteLine(helpMessage);
                        return false;
                }
            }

            //No invalid arguments or parameters was found - execution may continue.
            return true;
        }
    }
}
