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
            if (context.Users.Any())
            {
                ///<summary>
                ///SampleDataHandler creates a samples.json file by storing current database data in plaintext, in samples.json
                ///Use only if samples.json does not exist in Data folder and only sample data exists in the database
                ///</summary>
                sampleHandler.SerializeData(context, userManager);

                return;
            }

            SampleData sampleData = sampleHandler.DeserializeData();
            AddSampleUsers(context, userManager, sampleData.UserList);
            AddSamplePictograms(context, sampleData.PictogramList);
            AddSampleWeekAndWeekdays(context, sampleData.WeekdayList, sampleData.WeekList);
            AddSampleWeekTemplate(context, context.Pictograms.ToList(), context.Departments.ToList());
            AddSampleDepartments(context, sampleData.DepartmentList);

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
        //private static void AddSampleDepartments(GirafDbContext context, List<Department> departments)
        //{
        //    Console.WriteLine("Adding some sample data to the database.");
        //    Console.WriteLine("Adding departments.");

        //    foreach (var department in departments)
        //    {
        //        context.Departments.Add(department);
        //    }
        //    context.SaveChanges();
        //}

        private static void AddSampleDepartments(GirafDbContext context, List<SampleDepartment> sampleDeps)
        {
            Console.WriteLine("Adding departments.");

            List<Department> departments = new List<Department>();
            var sampleDepartments = sampleDeps;
            foreach (var sampleDepartment in sampleDepartments)
            {
                var department = new Department { Name = sampleDepartment.Name };
                context.Departments.Add(department);
            }
            context.SaveChanges();
        }

        //private static void AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, IList<GirafUser> users)
        //{
        //    Console.WriteLine("Adding users.");

        //    //users[0].UserIcon = null;
        //    //Note that the call to .Result is a dangerous way to run async methods synchonously and thus should not be used elsewhere!
        //    foreach (var user in users)
        //    {
        //        var x = userManager.CreateAsync(user, "password").Result;
        //    }

        //    // Add users to roles
        //    var a = userManager.AddToRoleAsync(users[0], GirafRole.Citizen).Result;
        //    a = userManager.AddToRoleAsync(users[1], GirafRole.Guardian).Result;
        //    a = userManager.AddToRoleAsync(users[2], GirafRole.SuperUser).Result;
        //    a = userManager.AddToRoleAsync(users[3], GirafRole.Department).Result;
        //    a = userManager.AddToRoleAsync(users[4], GirafRole.Citizen).Result;
        //}

        private static void AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, List<SampleGirafUser> userList)
        {
            Console.WriteLine("Adding users.");
            List<GirafUser> users = new List<GirafUser>();
            var sampleUsers = userList;

            foreach (var sampleUser in sampleUsers)
            {
                var gUser = (new GirafUser { UserName = sampleUser.Name, DepartmentKey = sampleUser.DepKey });

                foreach (string week in sampleUser.Weeks)
                {
                    gUser.WeekSchedule.Add(context.Weeks.First(w => w.Name == week));
                }
                userManager.CreateAsync(gUser, sampleUser.Password);
                userManager.AddToRoleAsync(gUser, sampleUser.Role);
                users.Add(gUser);
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
        }

        //private static void AddSampleWeekAndWeekdays(GirafDbContext context, IList<Pictogram> pictograms)
        //{
        //    Console.WriteLine("Adding weekdays to users");
        //    var weekdays = new Weekday[]
        //    {
        //        new Weekday(Days.Monday,
        //            new List<Pictogram> { pictograms[0], pictograms[1], pictograms[2], pictograms[3], pictograms[4] },
        //            new List<ActivityState>{Completed, Completed, Completed, Completed, Completed}),

        //        new Weekday(Days.Tuesday,
        //            new List<Pictogram> { pictograms[5], pictograms[6], pictograms[7], pictograms[8] },
        //            new List<ActivityState>{Completed, Active, Canceled, Completed}),

        //        new Weekday(Days.Wednesday,
        //            new List<Pictogram> { pictograms[9], pictograms[10], pictograms[11], pictograms[12], pictograms[13] },
        //            new List<ActivityState>{Completed, Active, Active, Active, Active}),

        //        new Weekday(Days.Thursday,
        //            new List<Pictogram> { pictograms[8], pictograms[6], pictograms[7], pictograms[5] },
        //            new List<ActivityState>{Active, Canceled, Active, Active}),

        //        new Weekday(Days.Friday,
        //            new List<Pictogram> { pictograms[0], pictograms[7]},
        //            new List<ActivityState>{Active, Active}),

        //        new Weekday(Days.Saturday,
        //            new List<Pictogram> { pictograms[8], pictograms[5]},
        //            new List<ActivityState>{Canceled, Canceled}),

        //        new Weekday(Days.Sunday,
        //            new List<Pictogram> { pictograms[3], pictograms[5]},
        //            new List<ActivityState>{Active, Active})
        //    };

        //    var sampleWeek = new Week(pictograms[0]);

        //    foreach (var day in weekdays)
        //    {
        //        context.Weekdays.Add(day);
        //        sampleWeek.UpdateDay(day);
        //    }

        //    sampleWeek.Name = "Normal Uge";
        //    var usr = context.Users.First(u => u.UserName == "Kurt");
        //    context.Weeks.Add(sampleWeek);
        //    usr.WeekSchedule.Add(sampleWeek);
        //    context.SaveChanges();
        //}

        private static void AddSampleWeekAndWeekdays(GirafDbContext context, List<SampleWeekday> sampleWeekdaysList, List<SampleWeek> sampleWeeksList)
        {
            Console.WriteLine("Adding weekdays to users");
            var sampleWeekdays = sampleWeekdaysList;
            var sampleWeek = sampleWeeksList[0];
            var week = new Week { Name = sampleWeek.Name, Thumbnail = context.Pictograms.First(p => p.Id == sampleWeek.PictKey) };

            foreach (var sampleWeekday in sampleWeekdays)
            {
                List<Pictogram> activityIcons = new List<Pictogram>();
                List<ActivityState> activityStates = new List<ActivityState>();

                foreach (string actTitle in sampleWeekday.ActivityIconTitles)
                {
                    foreach (Pictogram pic in context.Pictograms)
                    {
                        if (actTitle == pic.Title)
                        {
                            activityIcons.Add(pic);
                        }
                    }
                }

                foreach (string actState in sampleWeekday.ActivityStates)
                {
                    activityStates.Add((ActivityState)Enum.Parse(typeof(ActivityState), actState));
                    //activityStates.Add(actState);
                }

                var day = (new Weekday(sampleWeekday.Day, activityIcons, activityStates));
                context.Weekdays.Add(day);
                week.UpdateDay(day);
            }

            context.Weeks.Add(week);
            context.SaveChanges();
        }

        //private static void AddSampleWeekAndWeekdays(GirafDbContext context, List<SampleWeekday> sampleWeekdaysList, List<SampleWeek> sampleWeeksList)
        //{
        //    Console.WriteLine("Adding weekdays to users");
        //    var sampleWeekdays = sampleWeekdaysList; // Need source
        //    var sampleWeek = sampleWeeksList[0]; // Need source

        //    var week = new Week { Name = sampleWeek.Name, Thumbnail = context.Pictograms.First(p => p.Id == sampleWeek.PictKey) };
        //    foreach (var sampleWeekday in sampleWeekdays)
        //    {
        //        var activities = (from activity in sampleWeekday.ActivityIconTitles select (Activity)Enum.Parse(typeof(Activity), activity)).ToList();
        //        var weekday = new Weekday(sampleWeekday.Day, sampleWeekday.ActivityIconTitles, activities);
        //        context.Weekdays.Add(weekday);
        //        week.UpdateDay(weekday);
        //    }
        //    context.Weeks.Add(week);
        //    context.SaveChanges();
        //}

        private static void AddSampleWeekTemplate(GirafDbContext context, List<Pictogram> pictograms, List<Department> departments)
        {
            Console.WriteLine("Adding templates");
            var weekdays = new Weekday[]
            {
                new Weekday(Days.Monday,
                    new List<Pictogram> { pictograms[0], pictograms[1], pictograms[2], pictograms[3], pictograms[4] },
                    new List<ActivityState>{Active, Active, Active, Active, Active, }),

                new Weekday(Days.Tuesday,
                    new List<Pictogram> { pictograms[5], pictograms[6], pictograms[7], pictograms[8] },
                    new List<ActivityState>{Active, Active, Active, Active, }),

                new Weekday(Days.Wednesday,
                    new List<Pictogram> { pictograms[9], pictograms[10], pictograms[11], pictograms[12], pictograms[13] },
                    new List<ActivityState>{Active, Active, Active, Active, Active, }),

                new Weekday(Days.Thursday,
                    new List<Pictogram> { pictograms[8], pictograms[6], pictograms[7], pictograms[5] },
                    new List<ActivityState>{Active, Active, Active, Active, }),

                new Weekday(Days.Friday,
                    new List<Pictogram> { pictograms[0], pictograms[7]},
                    new List<ActivityState>{Active, Active, }),

                new Weekday(Days.Saturday,
                    new List<Pictogram> { pictograms[8], pictograms[5] },
                    new List<ActivityState>{Active, Active, }),

                new Weekday(Days.Sunday,
                    new List<Pictogram> { pictograms[3], pictograms[5] },
                    new List<ActivityState>{Active, Active, })
            };

            var sampleTemplate = new WeekTemplate(departments[0]);
            sampleTemplate.Name = "SkabelonUge";
            sampleTemplate.Thumbnail = pictograms[0];
            foreach (var day in weekdays)
            {
                context.Weekdays.Add(day);
                sampleTemplate.UpdateDay(day);
            }

            context.WeekTemplates.Add(sampleTemplate);
            context.SaveChanges();
        }
        #endregion
    }
}
