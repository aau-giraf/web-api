using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Setup
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    public static class DBInitializer
    {
        #region Properties

        public static SampleDataHandler SampleDataHandler { get; set; } = new SampleDataHandler();

        #endregion

        #region Methods

        /// <summary>
        /// Runs specificed initialisations before the API is started.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The API for managing GirafUsers.</param>
        /// <param name="pictogramCount">The number of sample pictograms to generate.</param>
        public static async Task Initialize(GirafDbContext context, UserManager<GirafUser> userManager, int pictogramCount)
        {
            CreatePictograms(pictogramCount);

            // Check if any data is in the database
            if (await context.Departments.AnyAsync())
            {
                ///<summary>
                ///SampleDataHandler creates a samples.json file by storing current database data in plaintext, in samples.json
                ///Use only if samples.json does not exist in Data folder and only sample data exists in the database.
                ///Passwords for users are written to the samples.json directly from the db, meaning they will be hashes. 
                ///If you want more writeable passwords, manually set them in the samples.json.
                ///</summary>
                //await SampleDataHandler.SerializeDataAsync(context, userManager);
                return;
            }
            // Get sample data
            SampleData sampleData = SampleDataHandler.DeserializeData();
            // Create departments
            var departments = await AddSampleDepartments(context, sampleData.DepartmentList);
            // Create users
            await AddSampleUsers(context, userManager, sampleData.UserList, departments);
            // Create pictograms
            var pictograms = await AddSamplePictograms(context, sampleData.PictogramList);
            // Create week and weekdays
            await AddSampleWeekAndWeekdays(context, sampleData.WeekList, sampleData.WeekdayList, sampleData.UserList, pictograms);
            // Create week templates
            await AddSampleWeekTemplate(context, sampleData.WeekTemplateList, sampleData.WeekdayList, sampleData.UserList, departments, pictograms);

            // Save changes
            await context.SaveChangesAsync();

            // Adding citizens to a Guardian
            foreach (var user in context.Users)
            {
                if (userManager.IsInRoleAsync(user, GirafRole.Guardian).Result)
                {
                    var citizens = user.Department.Members.Where(m => userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
                    user.AddCitizens(citizens);
                }
            }
            // Save changes
            await context.SaveChangesAsync();
        }

        #region Generating pictograms
        //Method for generating pictograms
        private static void CreatePictograms(int count)
        {
            System.Console.WriteLine($"Creating {count} pictograms");
            DirectoryInfo dir = Directory.CreateDirectory("../pictograms");
          
            /*using FontFamily family = new FontFamily("Arial");
            using Font font = new Font(
                family,
                48,
                FontStyle.Regular,
                GraphicsUnit.Pixel);*/

            for (int i = 1; i <= count; i++)
            {
                File.Copy("wwwroot/images/sample_pic.png",dir.FullName +$"/{i}.png",true);
                //using Image pictogram = DrawText(i.ToString(), font, Color.Black, Color.White);
                //pictogram.Save(Path.Combine(dir.FullName, $"{i}.png"), ImageFormat.Png); 
            }
        }

        private static Image DrawText(string text, Font font, Color textColor, Color backColor)
        {
            Image pictogram = new Bitmap(200, 200);
            using StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using Graphics drawing = Graphics.FromImage(pictogram);
            using Brush textBrush = new SolidBrush(textColor);

            drawing.Clear(backColor);
            drawing.DrawString(text, font, textBrush, RectangleF.FromLTRB(0, 0, 200, 200), format);
            drawing.Save();

            return pictogram;
        }
        #endregion

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

        private static async Task<List<Department>> AddSampleDepartments(GirafDbContext context, List<SampleDepartment> sampleDeps)
        {
            Console.WriteLine("Adding departments.");
            List<Department> departments = new List<Department>();

            foreach (SampleDepartment sampleDepartment in sampleDeps)
            {
                Department department = new Department { Name = sampleDepartment.Name };
                departments.Add(department);
                await context.Departments.AddAsync(department);
            }
            await context.SaveChangesAsync();
            return departments;
        }
        private static async Task AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, List<SampleGirafUser> sampleUsers, List<Department> departments123)
        {
            Console.WriteLine("Adding users.");
            List<GirafUser> users = new List<GirafUser>();
            List<Department> departments = await context.Departments.ToListAsync();

            foreach (var sampleUser in sampleUsers)
            {
                GirafUser user = new GirafUser
                {
                    UserName = sampleUser.Name,
                    DisplayName = sampleUser.DisplayName,
                    DepartmentKey = departments.FirstOrDefault(d => d.Name == sampleUser.DepartmentName).Key
                };

                var x = await userManager.CreateAsync(user, sampleUser.Password);
                if (x.Succeeded)
                {
                    var a = await userManager.AddToRoleAsync(user, sampleUser.Role);
                    if (!a.Succeeded)
                        throw new Exception("Failed to add role " + sampleUser.Role + " to user " + user.UserName);
                }
                else
                    throw new WarningException("Failed to create user " + user.UserName + " in usermanager");
                users.Add(user);
            }
        }
        private static async Task<List<Pictogram>> AddSamplePictograms(GirafDbContext context, List<SamplePictogram> samplePictogramsList)
        {
            System.Console.WriteLine("Adding pictograms.");
            List<Pictogram> pictograms = new List<Pictogram>();
            foreach (var samplePict in samplePictogramsList)
            {
                var pictogram = new Pictogram(samplePict.Title, (AccessLevel)Enum.Parse(typeof(AccessLevel), samplePict.AccessLevel), samplePict.ImageHash);
                pictogram.LastEdit = DateTime.Now;
                await context.AddAsync(pictogram);
                pictograms.Add(pictogram);
            }
            return pictograms;
        }
        private static async Task AddSampleWeekAndWeekdays(GirafDbContext context, List<SampleWeek> sampleWeeks, List<SampleWeekday> sampleWeekdays, List<SampleGirafUser> sampleUsers, List<Pictogram> pictograms)
        {
            Console.WriteLine("Adding weekdays to users");
            Pictogram thumbNail = null;
            List<Week> weekList = new List<Week>();

            foreach (SampleWeek sampleWeek in sampleWeeks)
            {
                foreach (Pictogram pic in pictograms)
                {
                    if (pic.Title == sampleWeek.ImageTitle)
                    {
                        thumbNail = pic;
                    }
                }

                Week week = new Week { Name = sampleWeek.Name, Thumbnail = thumbNail };
                await AddDaysToWeekAndContext(sampleWeekdays, week, context, pictograms);
                await context.Weeks.AddAsync(week);
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
        private static async Task AddSampleWeekTemplate(GirafDbContext context, List<SampleWeekTemplate> sampleTemplates, List<SampleWeekday> sampleWeekdays,
            List<SampleGirafUser> sampleUsers, List<Department> departments, List<Pictogram> pictograms)
        {
            Console.WriteLine("Adding templates");
            Pictogram thumbNail = null;

            foreach (SampleWeekTemplate sampleTemplate in sampleTemplates)
            {
                foreach (Pictogram pic in pictograms)
                {
                    if (pic.Title == sampleTemplate.ImageTitle)
                    {
                        thumbNail = pic;
                    }
                }

                var template = new WeekTemplate
                {
                    Name = sampleTemplate.Name,
                    Thumbnail = thumbNail,
                    Department = departments.First(d => d.Name == sampleTemplate.DepartmentName)
                };

                await AddDaysToWeekAndContext(sampleWeekdays, template, context, pictograms);
                await context.WeekTemplates.AddAsync(template);

                foreach (GirafUser user in context.Users)
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if ((userWeek == template.Name) && (user.UserName == sampleUser.Name))
                            {
                                user.WeekSchedule.Add(await context.Weeks.FirstAsync(w => w.Name == userWeek));
                            }
                        }
                    }
                }
            }
        }
        private static async Task AddDaysToWeekAndContext(List<SampleWeekday> sampleDays, WeekBase week, GirafDbContext context, List<Pictogram> pictograms)
        {
            foreach (var sampleDay in sampleDays)
            {
                Days day = sampleDay.Day;
                List<List<Pictogram>> picts = new List<List<Pictogram>>();
                Pictogram pic = null;

                foreach (string actIcon in sampleDay.ActivityIconTitles)
                {
                    foreach (Pictogram pict in pictograms)
                    {
                        if (pict.Title == actIcon)
                        {
                            pic = pict;
                        }
                    }
                    picts.Add(new List<Pictogram>{ pic });
                }

                List<ActivityState> activityStates = (from activityState in sampleDay.ActivityStates select (ActivityState)Enum.Parse(typeof(ActivityState), activityState)).ToList<ActivityState>();
                Weekday weekDay = new Weekday(day, picts, activityStates);
                await context.Weekdays.AddAsync(weekDay);
                week.UpdateDay(weekDay);
            }
        }

        #endregion

        #endregion
    }
}
