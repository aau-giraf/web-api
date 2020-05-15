using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest
{
    /// <summary>
    /// A class that may be used to pass program arguments
    /// </summary>
    public class ProgramArgumentParser
    {
        /// <summary>
        /// A list of arguments that are allowed by the application.
        /// </summary>
        private const string _options =
                          "\t--port=integer\t\t| Specify which port to host the server on, defaults to 5000.\n" +
                          "\t--list\t\t\t| List options\n" +
                          "\t--sample-data\t\t| Tells the rest-api to generate some sample data. This only works on an empty database.\n" +
                          "\t--pictograms=integer\t| Specify how many sample pictograms to generate. Default is 200. Only works when --sample-data is set.\n" +
                          "\t--logfile=string\t| Toggles logging to a file, the string specifies the path to the file relative to the working directory.";
        /// <summary>
        /// A short help message telling the user how to see all program arguments.
        /// </summary>
        private const string _helpMessage = "\tRun with --list to list options";

        /// <summary>
        /// A delegate that defines the structure of program argument handler methods.
        /// </summary>
        /// <param name="parameter">A series of strings for the parameters of each argument.</param>
        private delegate void ProgramArgumentHandler(params string[] parameter);
        /// <summary>
        /// A dictionary for storing each configuration method under a given name.
        /// </summary>
        private Dictionary<string, ProgramArgumentHandler> programArgumentDictionary;

        /// <summary>
        /// Constructs a new ProgramArgumentParser, that can parse program arguments. 
        /// It also adds all allowed arguments to the dictionary.
        /// </summary>
        public ProgramArgumentParser()
        {
            programArgumentDictionary = new Dictionary<string, ProgramArgumentHandler>();
            programArgumentDictionary["--port"] = programArgumentPort;
            programArgumentDictionary["--list"] = (x) => Console.WriteLine(_options);
            programArgumentDictionary["--sample-data"] = (x) =>
            {
                Console.WriteLine("\tEnabled sample data option.");
                ProgramOptions.GenerateSampleData = true;
            };
            programArgumentDictionary["--pictograms"] = programArgumentPictograms;
            programArgumentDictionary["--logfile"] = programArgumentLogfile;
        }

        /// <summary>
        /// Run over the array of program arguments, apply their options and check that they are valid.
        /// </summary>
        /// <param name="args">An array of program arguments.</param>
        /// <returns>True if all the arguments were valid and false otherwise.</returns>
        public bool CheckProgramArguments(string[] args)
        {
            if (args == null) {
                throw new System.ArgumentNullException(args + " is null");
            }
            //Check if no arguments are specified and run in default configuration if so.
            if (args.Length == 0)
            {
                Console.WriteLine(new string("\tNo program arguments were found - running in default configuration."));
                Console.WriteLine(new string(_helpMessage));
                return true;
            }

            //Try to apply the changes of each argument
            try
            {
                foreach (var arg in args)
                {
                    string[] split = arg.Split('=');
                    string argument = split[0];
                    string parameter = split.Length > 1 ? split[1] : null;

                    programArgumentDictionary[argument](parameter);
                }
                return true;
            }
            //An argument as invalid, display the error message and return false.
            catch (Exception e)
            {
                Console.WriteLine(new string(_options));
                Console.WriteLine(new string("\n\nAn exception occurred. Please check the arguments you have specified, " +
                    "the list of supported options are shown above for you convenience.\n" +
                    "The exception was:\n"));
                Console.WriteLine(e.Message);
                return false;
                throw;
            }
        }

        /// <summary>
        /// Attempts to configure which port to host the server on.
        /// </summary>
        /// <param name="portString">A string representation of an integer, denoting the desired port of the server.</param>
        private void programArgumentPort(params string[] portString)
        {
            try
            {
                var p = Int16.Parse(portString[0], System.Globalization.NumberFormatInfo.InvariantInfo);
                Console.WriteLine("\tSetting the port to " + p.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                ProgramOptions.Port = p;
            }
            catch
            {
                throw new ArgumentException("\tERROR: Invalid port parameter was specified, expected an integer, but found "
                    + portString[0]);
            }
        }

        /// <summary>
        /// Attempts to configure the number of sample pictograms to generate.
        /// </summary>
        /// <param name="argument">A string representation of an integer, denoting the desired number of sample pictograms to generate.</param>
        private void programArgumentPictograms(params string[] argument)
        {
            try
            {
                var p = Int32.Parse(argument[0], System.Globalization.NumberFormatInfo.InvariantInfo);
                ProgramOptions.Pictograms = p;
            }
            catch
            {
                throw new ArgumentException("\tERROR: Invalid pictograms parameter was specified, expected an integer, but found "
                    + argument[0]);
            }
        }

        /// <summary>
        /// Specifies that the server should utilize file-logging to the given file.
        /// </summary>
        /// <param name="filename">The name of the file to log to. All files will be placed in wwwroot/Logs.</param>
        private void programArgumentLogfile(params string[] filename)
        {
            if (String.IsNullOrWhiteSpace(filename[0]))
                throw new ArgumentException("\tERROR: Invalid parameter specified for --logfile, expected a filename, but found "
                    + filename);
            Console.WriteLine("\tEnabling file-logging on the path " + Path.Combine(ProgramOptions.LogDirectory, filename[0]));
            ProgramOptions.LogToFile = true;
            ProgramOptions.LogFilepath = filename[0];
        }
    }
}
