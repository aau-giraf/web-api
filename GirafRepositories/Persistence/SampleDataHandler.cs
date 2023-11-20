using GirafRest.Data;
using GirafRest.Data.Samples;
using Newtonsoft.Json;

namespace GirafRepositories.Persistence
{
    public class SampleDataHandler
    {
        private readonly string jsonFile = $"{Directory.GetCurrentDirectory()}" +
            $"{Path.DirectorySeparatorChar}" +
            $"Data" +
            $"{Path.DirectorySeparatorChar}";
        private readonly string pictogramsFile = $"{Directory.GetCurrentDirectory()}" +
            $"{Path.DirectorySeparatorChar}" +
            $"Data" +
            $"{Path.DirectorySeparatorChar}pictograms.json";
        private readonly string rolesFile = $"{Directory.GetCurrentDirectory()}" +
           $"{Path.DirectorySeparatorChar}" +
           $"Data" +
           $"{Path.DirectorySeparatorChar}roles.json";

        public SampleDataHandler(string path)
        {
            jsonFile += path;
        }

        public SampleData DeserializeData()
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<SampleData>(jsonString);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }

        public List<SamplePictogram> ReadSamplePictograms() {
            try
            {
                string jsonString = File.ReadAllText(pictogramsFile);
                return JsonConvert.DeserializeObject<List<SamplePictogram>>(jsonString);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }

        public List<string> ReadSampleRoles()
        {
            try
            {
                string jsonString = File.ReadAllText(rolesFile);
                return JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }
    }
}
