﻿

using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


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

        public void SerializeData(GirafDbContext context, UserManager<GirafUser> userManager)
        {
            SampleData data = new SampleData();

            List<GirafUser> userList = (from user in context.Users select user).ToList();
            List<Department> departmentList = (from dep in context.Departments select dep).ToList();
            List<Pictogram> pictogramList = (from pic in context.Pictograms select pic).ToList();
            List<Activity> activityList = (from act in context.Activities select act).ToList();
            List<Weekday> weekdayList = (from day in context.Weekdays select day).ToList();
            List<Week> weekList = (from week in context.Weeks select week).ToList();
            List<WeekTemplate> weekTemplateList = (from weekTemplate in context.WeekTemplates select weekTemplate).ToList();


            foreach (GirafUser user in userList)
            {
                List<string> weekStrings = new List<string>();
                foreach (Week week in user.WeekSchedule)
                {
                    weekStrings.Add(week.Name);
                }

                if (user.Department == null)
                {
                    data.UserList.Add(new SampleGirafUser(user.UserName, 0, userManager.GetRolesAsync(user).Result[0], weekStrings, user.PasswordHash));
                }
                else
                {
                    data.UserList.Add(new SampleGirafUser(user.UserName, user.Department.Key, userManager.GetRolesAsync(user).Result[0], weekStrings, user.PasswordHash));
                }
            }


            foreach (Department dep in departmentList)
            {
                data.DepartmentList.Add(new SampleDepartment(dep.Name));
            }


            foreach (Pictogram pic in pictogramList)
            {
                data.PictogramList.Add(new SamplePictogram(pic.Title, pic.AccessLevel.ToString(), pic.ImageHash));
            }


            foreach (Weekday day in weekdayList)
            {
                List<string> actIconTitles = new List<string>();
                List<string> actStates = new List<string>();

                foreach (Activity act in activityList)
                {
                    if (day.Id == act.OtherKey)
                    {
                        actIconTitles.Add(act.Pictogram.Title);
                        actStates.Add(act.State.ToString());
                    }
                }

                data.WeekdayList.Add(new SampleWeekday(day.Day, actIconTitles, actStates));
            }


            foreach (Week week in weekList)
            {
                long thumbKey = 0;
                foreach (Pictogram pic in context.Pictograms)
                {
                    if (pic.Id == week.Thumbnail.Id)
                    {
                        thumbKey = pic.Id;
                    }
                }
                data.WeekList.Add(new SampleWeek(week.Name, thumbKey));
            }


            foreach (WeekTemplate weekTemp in context.WeekTemplates)
            {
                data.WeekTemplateList.Add(new SampleWeekTemplate(weekTemp.Name, weekTemp.ThumbnailKey, weekTemp.DepartmentKey));
            }



            string jsonSamples = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });

            File.WriteAllText(jsonFile, jsonSamples);
        }
    }

    public class SampleData
    {
        public List<SampleGirafUser> UserList { get; set; }
        public List<SampleDepartment> DepartmentList { get; set; }
        public List<SamplePictogram> PictogramList { get; set; }
        public List<SampleWeekday> WeekdayList { get; set; }
        public List <SampleWeek> WeekList { get; set; }
        public List <SampleWeekTemplate> WeekTemplateList { get; set; }

        public SampleData()
        {
            UserList = new List<SampleGirafUser>();
            DepartmentList = new List<SampleDepartment>();
            PictogramList = new List<SamplePictogram>();
            WeekdayList = new List<SampleWeekday>();
            WeekList = new List<SampleWeek>();
            WeekTemplateList = new List<SampleWeekTemplate>();
        }
    }
}
