

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

        public void SerializeData(GirafDbContext context)
        {
            SampleData data = new SampleData();

            data.userList = (from user in context.Users select user).ToList();
            data.departmentList = (from dep in context.Departments select dep).ToList();
            data.pictogramList = (from pic in context.Pictograms select pic).ToList();
            data.weekList = (from user in context.Users select user.WeekSchedule.ToList()).ToList();
            //data.activityList = (from d in (from a in data.weekList select a) select d.).ToList();

            List<Activity> tempActList = new List<Activity>();

            foreach (List<Week> weeks in data.weekList)
            {
                foreach (Week week in weeks)
                {
                    foreach (Weekday day in week.Weekdays.ToList())
                    {
                        foreach (Activity act in day.Activities)
                        {
                            tempActList.Add(act);
                            Console.WriteLine(tempActList.ToString());
                        }
                    }
                }
            }

            data.activityList = tempActList;

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
        public List<List<Week>> weekList { get; set; }
        public List<Activity> activityList { get; set; }
    }
}
