﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafWebApi.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace GirafWebApi
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

        public static DbOption DbOption;
        /// <summary>
        /// Return value for the Main-function in case it has been run without arguments.
        /// </summary>
        private const int ERROR_BAD_ARGUMENTS = 0xA0;
        /// <summary>
        /// Return value for the Main-function in case it has succesfully completed.
        /// </summary>
        private const int SUCCESS = 0x0;
        /// <summary>
        /// Program argument for running the server locally.
        /// </summary>
        private const string RUN_WITH_LOCALDB = "-localdb";
        /// <summary>
        /// Program argument for deploying the server.
        /// </summary>
        private const string RUN_DEPLOYMENT = "-deploy";

        /// <summary>
        /// Main function of the Giraf REST-api.
        /// </summary>
        /// <param name="args">Program arguments (see <see cref="RUN_WITH_LOCALDB"/> and <see cref="RUN_DEPLOYMENT"/> for explanation)</param>
        /// <returns>An exit code (see <see cref="ERROR_BAD_ARGUMENTS"/> and <see cref="SUCCESS"/> for explanation)</returns>
        public static int Main(string[] args)
        {
            Console.Title = "Giraf REST-api Console";

            //Display a message for the user in case no arguments were specified.
            if(args.Length == 0) {
                System.Console.WriteLine("Welcome to Giraf REST Server.");
                System.Console.WriteLine("You have not specified any arguments, please use the following:");
                System.Console.WriteLine($"   {RUN_WITH_LOCALDB}       : Runs a local SQLite db for debugging");
                System.Console.WriteLine($"   {RUN_DEPLOYMENT} #path# : Deploys the server #path# must be the path to a web.config file containing a ConnectionString for the MariaDB.");
                System.Console.WriteLine("Now defaulting to localdb");
                DbOption = DbOption.SQLite;
            }

            //Build the host from the given arguments.
            IWebHost host = null;
            if(args[0].Equals(RUN_WITH_LOCALDB)) {
                host = ConfigureHost();
                DbOption = DbOption.SQLite;
            }
            else if(args[0].Equals(RUN_DEPLOYMENT)) {
                host = ConfigureHost();
                ConfigurationFilePath = args[1];
                DbOption = DbOption.SQL;
            }

            //Attempt to run the host, else display an error and abort.
            if(host != null)
                host.Run();
            else {
                System.Console.WriteLine("Your argument was not recognized, aborting.");
                return ERROR_BAD_ARGUMENTS;
            }

            return SUCCESS;
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
                .UseUrls("http://localhost:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            return host;
        }
    }
}
