

using GirafRest.Data;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace GirafRest.Setup
{
    public class SampleDataHandler
    {
        private readonly string jsonFile = "..\\GirafRest\\Data\\samples.json";

        public SampleDataHandler(){}
        public SampleDataHandler(string path)
        {
            jsonFile = path;
        }

        public SampleData initSampleData()
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

        public void SerializeData(GirafDbContext context)
        {
            SampleData data = new SampleData();

            data.userList = (from user in context.Users select user).ToList();
            data.departmentList = (from dep in context.Departments select dep).ToList();
            data.pictogramList = (from pic in context.Pictograms select pic).ToList();

            string jsonSamples = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            });

            File.WriteAllText(jsonFile, jsonSamples);
        }
    }

    public class SampleData
    {
        public List<GirafUser> userList { get; set; }
        public List<Department> departmentList { get; set; }
        public List<Pictogram> pictogramList { get; set; }
    }
}
