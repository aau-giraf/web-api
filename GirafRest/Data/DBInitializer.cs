using System;
using System.Collections.Generic;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using static GirafRest.Models.ActivityState;

namespace GirafRest.Setup
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    public class DBInitializer
    {

        public static SampleDataHandler sampleHandler = new SampleDataHandler();

        public static void Initialize(GirafDbContext context, UserManager<GirafUser> userManager)
        {
            // Check if any data is in the database
            if (context.Departments.Any())
            {
                ///<summary>
                ///SampleDataHandler creates a samples.json file by storing current database data in plaintext, in samples.json
                ///Use only if samples.json does not exist in Data folder and only sample data exists in the database
                ///</summary>
                sampleHandler.SerializeData(context, userManager);

                return;
            }

            SampleData sampleData = sampleHandler.DeserializeData();
            var departments = AddSampleDepartments(context, sampleData.DepartmentList);
            AddSampleUsers(context, userManager, sampleData.UserList, departments);
            AddSamplePictograms(context, sampleData.PictogramList);
            AddSampleWeekAndWeekdays(context, sampleData.WeekList, sampleData.WeekdayList, sampleData.UserList);
            AddSampleWeekTemplate(context, sampleData.WeekTemplateList, sampleData.WeekdayList, sampleData.UserList, departments);

            context.SaveChanges();

            //Adding citizens to Guardian
            foreach (var user in context.Users)
            {
                if (userManager.IsInRoleAsync(user, GirafRole.Guardian).Result)
                {
                    var citizens = user.Department.Members
                    .Where(m => userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
                    user.AddCitizens(citizens);
                }
            }
            context.SaveChanges();
        }

        #region Ownership sample methods
        private static void AddPictogramsToUser(GirafUser user, params Pictogram[] pictograms)
        {
            System.Console.WriteLine("Adding pictograms to " + user.UserName);
            foreach (var pict in pictograms)
            {
                if (pict.AccessLevel != AccessLevel.PRIVATE)
                    throw new InvalidOperationException($"You may only add private pictograms to users." +
                        " Pictogram id: {pict.Id}, AccessLevel: {pict.AccessLevel}.");
                new UserResource(user, pict);
            }
        }

        private static void AddPictogramToDepartment(Department department, params Pictogram[] pictograms)
        {
            Console.WriteLine("Adding pictograms to " + department.Name);
            foreach (var pict in pictograms)
            {
                if (pict.AccessLevel != AccessLevel.PROTECTED)
                    throw new InvalidOperationException($"You may only add protected pictograms to department." +
                        " Pictogram id: {pict.Id}, AccessLevel: {pict.AccessLevel}.");
                new DepartmentResource(department, pict);
            }
        }
        #endregion
        #region Sample data methods
        private static List<Department> AddSampleDepartments(GirafDbContext context, List<SampleDepartment> sampleDeps)
        {
            Console.WriteLine("Adding departments.");
            List<Department> departments = new List<Department>();

            foreach (SampleDepartment sampleDepartment in sampleDeps)
            {
                var department = new Department { Name = sampleDepartment.Name };
                context.Departments.Add(department);
                departments.Add(department);
            }
            context.SaveChanges();
            return departments;
        }

        private static void AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, List<SampleGirafUser> userList, List<Department> departments)
        {
            Console.WriteLine("Adding users.");
            List<GirafUser> users = new List<GirafUser>();
            var sampleUsers = userList;

            foreach (var sampleUser in sampleUsers)
            {
                GirafUser user = (new GirafUser { UserName = sampleUser.Name, 
                    Department = departments.First(d => d.Name == sampleUser.DepartmentName) });

                var x = userManager.CreateAsync(user, sampleUser.Password).Result;
                var a = userManager.AddToRoleAsync(user, sampleUser.Role).Result;
                users.Add(user);
            }
        }

        private static void AddSamplePictograms(GirafDbContext context, List<SamplePictogram> samplePictogramsList)
        {
            System.Console.WriteLine("Adding pictograms.");
            List<Pictogram> pictograms = new List<Pictogram>();
            foreach (var samplePict in samplePictogramsList)
            {
                var pictogram = new Pictogram(samplePict.Title, (AccessLevel)Enum.Parse(typeof(AccessLevel), samplePict.AccessLevel), samplePict.ImageHash);
                pictogram.LastEdit = DateTime.Now;
                context.Add(pictogram);
                pictograms.Add(pictogram);
            }
            context.SaveChanges();
        }

        private static void AddSampleWeekAndWeekdays(GirafDbContext context, List<SampleWeek> sampleWeeks, List<SampleWeekday> sampleWeekdays, List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding weekdays to users");
            Pictogram thumbNail = null;
            List<Week> weekList = new List<Week>();

            foreach (SampleWeek sampleWeek in sampleWeeks)
            {
                foreach (Pictogram pic in context.Pictograms)
                {
                    if (pic.Title == sampleWeek.ImageTitle)
                    {
                        thumbNail = pic;
                    }
                }

                Week week = new Week { Name = sampleWeek.Name, Thumbnail = thumbNail };
                AddDaysToWeekAndContext(sampleUsers, sampleWeekdays, week, context);
                context.Weeks.Add(week);
                weekList.Add(week);

                foreach (GirafUser user in context.Users)
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if ((userWeek == week.Name) && (user.UserName == sampleUser.Name))
                            {
                                user.WeekSchedule.Add(weekList.First(w => w.Name == userWeek));
                            }
                        }
                    }
                }
            }
        }

        private static void AddSampleWeekTemplate(GirafDbContext context, List<SampleWeekTemplate> sampleTemplates, List<SampleWeekday> sampleWeekdays, 
            List<SampleGirafUser> sampleUsers, List<Department> departments)
        {
            Console.WriteLine("Adding templates");
            Pictogram thumbNail = null;

            foreach (SampleWeekTemplate sampleTemplate in sampleTemplates)
            {
                foreach (Pictogram pic in context.Pictograms)
                {
                    if (pic.Title == sampleTemplate.ImageTitle)
                    {
                        thumbNail = pic;
                    }
                }

                var template = new WeekTemplate { Name = sampleTemplate.Name, Thumbnail = thumbNail, 
                    Department = departments.First(d => d.Name == sampleTemplate.DepartmentName) };

                AddDaysToWeekAndContext(sampleUsers, sampleWeekdays, template, context);
                context.WeekTemplates.Add(template);

                foreach (GirafUser user in context.Users)
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if ((userWeek == template.Name) && (user.UserName == sampleUser.Name))
                            {
                                user.WeekSchedule.Add(context.Weeks.First(w => w.Name == userWeek));
                            }
                        }
                    }
                }
            }
        }


        private static void AddDaysToWeekAndContext(List<SampleGirafUser> sampleUsers, List<SampleWeekday> sampleDays, WeekBase week, GirafDbContext context)
        {
            foreach (var sampleDay in sampleDays)
            {
                Days day = sampleDay.Day;
                List<Pictogram> pictograms = new List<Pictogram>();
                Pictogram pic = null;

                foreach (string actIcon in sampleDay.ActivityIconTitles)
                {
                    foreach (Pictogram pict in context.Pictograms)
                    {
                        if (pict.Title == actIcon)
                        {
                            pic = pict;
                        }
                    }
                    pictograms.Add(pic);
                }

                List<ActivityState> activityStates = (from activityState in sampleDay.ActivityStates select (ActivityState)Enum.Parse(typeof(ActivityState), activityState)).ToList<ActivityState>();
                Weekday weekDay = new Weekday(day, pictograms, activityStates);
                context.Weekdays.Add(weekDay);
                week.UpdateDay(weekDay);
            }
        }
        #endregion
    }
}
