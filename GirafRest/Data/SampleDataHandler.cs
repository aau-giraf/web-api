using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data.Samples;

namespace GirafRest.Data
{
    public class SampleDataHandler
    {
        private readonly string jsonFile = $"{Directory.GetCurrentDirectory()}" +
            $"{Path.DirectorySeparatorChar}" +
            $"Data" +
            $"{Path.DirectorySeparatorChar}";

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

        /// <summary>
        /// Serialize database data and save it into a file
        /// </summary>
        /// <param name="context">Context for the database</param>
        /// <param name="userManager">ASP.NET Core user manager</param>
        /// <returns>A task</returns>
        public async Task SerializeDataAsync(GirafDbContext context, UserManager<GirafUser> userManager)
        {
            SampleData data = new SampleData();
            
            // Giraf users
            List<GirafUser> userList = await context.Users.ToListAsync();
            // Departments
            List<Department> departmentList = await context.Departments.ToListAsync();
            // Pictograms
            List<Pictogram> pictogramList = await context.Pictograms.ToListAsync();
            // Activities
            List<Activity> activityList = await context.Activities.Include(x => x.Pictograms)
                                                                  .ToListAsync();
            // Weekdays
            List<Weekday> weekdayList = await context.Weekdays.ToListAsync();
            // Week
            List<Week> weekList = await context.Weeks.ToListAsync();
            // Week template
            List<WeekTemplate> weekTemplateList = await context.WeekTemplates.ToListAsync();
            // Convert users into sample data
            foreach (GirafUser user in userList)
            {
                List<string> weekStrings = new List<string>();
                foreach (Week week in user.WeekSchedule)
                    weekStrings.Add(week.Name);

                IList<string> roles = await userManager.GetRolesAsync(user);

                if (!roles.Any())
                {
                    roles = new List<string>();
                    roles.Add(GirafRole.Citizen);
                }

                var password = "password";
                if (user.Department == null)
                    data.UserList.Add(new SampleGirafUser(user.UserName, user.DisplayName, "", roles[0], weekStrings, password));
                else
                    data.UserList.Add(new SampleGirafUser(user.UserName, user.DisplayName, user.Department.Name, roles[0], weekStrings, password));
            }
            // Convert departments into sample data
            foreach (Department dep in departmentList)
                data.DepartmentList.Add(new SampleDepartment(dep.Name));
            // Convert pictograms into sample data
            foreach (Pictogram pic in pictogramList)
                data.PictogramList.Add(new SamplePictogram(pic.Title, pic.AccessLevel.ToString(), pic.ImageHash));
            // Convert weekdays into sample data
            foreach (Weekday day in weekdayList)
            {
                List<string> actIconTitles = new List<string>();
                List<string> actStates = new List<string>();

                foreach (Activity act in activityList)
                {
                    if (day.Id == act.OtherKey)
                    {
                        actIconTitles.Add(act.Pictograms.First().Pictogram.Title);
                        actStates.Add(act.State.ToString());
                    }
                }

                data.WeekdayList.Add(new SampleWeekday(day.Day, actIconTitles, actStates));
            }
            // Convert weeks into sample data
            foreach (Week week in weekList)
            {
                string thumbTitle = "0";
                foreach (Pictogram pic in pictogramList)
                {
                    if (week.Thumbnail == null)
                    {
                        week.Thumbnail = pictogramList[0];
                    }
                    if (pic.Title == week.Thumbnail.Title)
                    {
                        thumbTitle = pic.Title;
                    }
                }
                data.WeekList.Add(new SampleWeek(week.Name, thumbTitle));
            }
            // Convert week templates into sample data
            foreach (WeekTemplate weekTemp in weekTemplateList)
            {
                if (weekTemp.Thumbnail == null)
                {
                    weekTemp.Thumbnail = pictogramList[0];
                }
                if (weekTemp.Department == null)
                {
                    weekTemp.Department = departmentList[0];
                }
                data.WeekTemplateList.Add(new SampleWeekTemplate(weekTemp.Name, weekTemp.Thumbnail.Title, weekTemp.Department.Name));
            }
            // Save sample data into file
            string jsonSamples = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(jsonFile, jsonSamples);
        }
    }
}
