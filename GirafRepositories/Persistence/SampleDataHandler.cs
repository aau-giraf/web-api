using GirafEntities.Samples;
using Newtonsoft.Json;

namespace GirafRepositories.Persistence
{
    public class SampleDataHandler
    {
        //If these paths are changed, the Dockerfile needs to be updated.
        private readonly string jsonFile = $"{Directory.GetCurrentDirectory()}" +
                                           $"{Path.DirectorySeparatorChar}";
        private readonly string pictogramsFile = $"../GirafRepositories/Persistence/pictograms.json";
        private readonly string rolesFile = $"../GirafRepositories/Persistence/roles.json";

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
