using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest
{
    /// <summary>
    /// A static class for storing all options for the rest api.
    /// </summary>
    public static class ProgramOptions
    {
        /// <summary>
        /// An option that indicates whether the user has specified that sample data should be added to the database on start up.
        /// </summary>
        public static bool GenerateSampleData = false;

        /// <summary>
        /// A field for storing the port on which to host the server.
        /// </summary>
        public static Int16 Port = 5000;

        /// <summary>
        /// Indicates if the server should utilize file logging.
        /// </summary>
        public static bool LogToFile = false;
        /// <summary>
        /// The file path of the file to log to.
        /// </summary>
        public static string LogFilepath = "";

        /// <summary>
        /// The directory where log-files should be placed.
        /// </summary>
        public static readonly string LogDirectory = Path.Combine("wwwroot", "DebugLogs");
    }
}
