using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GirafRest.Setup
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    static public class DBInitializer
    {
        private static SampleDataHandler sampleHandler = new SampleDataHandler();

        /// <summary>
        /// Runs specificed initialisations before the API is started.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userManager">The API for managing GirafUsers.</param>
        /// <param name="pictogramCount">The number of sample pictograms to generate.</param>
        public static void Initialize(GirafDbContext context, UserManager<GirafUser> userManager, int pictogramCount)
        {
            if (context == null)
                throw new System.ArgumentNullException(nameof(context) + new string(" is null"));

            if (userManager == null)
                throw new System.ArgumentNullException(nameof(userManager) + new string(" is null"));

            CreatePictograms(pictogramCount);

            // Check if any data is in the database
            if (context.Departments.Any())
            {
                
                //<summary>
                //SampleDataHandler creates a samples.json file by storing current database data in plaintext, in samples.json
                //Use only if samples.json does not exist in Data folder and only sample data exists in the database.
                //Passwords for users are written to the samples.json directly from the db, meaning they will be hashes. 
                //If you want more writeable passwords, manually set them in the samples.json.
                //</summary>
                System.Threading.Tasks.Task task = sampleHandler.SerializeDataAsync(context, userManager);

                return;
            }

            SampleData sampleData = sampleHandler.DeserializeData();
            var departments = AddSampleDepartments(context, sampleData.DepartmentList);
            AddSampleUsers(context, userManager, sampleData.UserList);
            if (userManager == null)
                throw new System.ArgumentNullException(nameof(userManager) + new string(" is null"));
            var pictograms = AddSamplePictograms(context, sampleData.PictogramList);
            AddSampleWeekAndWeekdays(context, sampleData.WeekList, sampleData.WeekdayList, sampleData.UserList, pictograms);

            AddSampleWeekTemplate(context, sampleData.WeekTemplateList, sampleData.WeekdayList, sampleData.UserList, departments, pictograms);

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
        
        #region Generating pictograms
        private static void CreatePictograms(int count)
        {
            System.Console.WriteLine($"Creating {count} pictograms");
            DirectoryInfo dir = Directory.CreateDirectory("../pictograms");

            using FontFamily family = new FontFamily("Arial");
            using Font font = new Font(
                family,
                48,
                FontStyle.Regular,
                GraphicsUnit.Pixel);

            for (int i = 1; i <= count; i++)
            {
                using Image pictogram = DrawText(i.ToString("G", System.Globalization.CultureInfo
                .InvariantCulture), font, Color.Black, Color.White);
                pictogram.Save(Path.Combine(dir.FullName, $"{i}.png"), ImageFormat.Png); 
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
            String temp1 = new String("You may only add private pictograms to users." +
                        " Pictogram id: {pict.Id}, AccessLevel: {pict.AccessLevel}.");
            System.Console.WriteLine("Adding pictograms to " + user.UserName);
            foreach (var pict in pictograms)
            {
                if (pict.AccessLevel != AccessLevel.PRIVATE)
                    throw new InvalidOperationException(temp1);
                
            }
        }

        private static void AddPictogramToDepartment(Department department, params Pictogram[] pictograms)
        {
            String str1 = new String("Adding pictograms to");
            String exception = new String("(You may only add protected pictograms to department." +
                        " Pictogram id: {pict.Id}, AccessLevel: {pict.AccessLevel}.");
            Console.WriteLine(str1 + department.Name);
            foreach (var pict in pictograms)
            {
                if (pict.AccessLevel != AccessLevel.PROTECTED)
                    throw new InvalidOperationException(exception);
            }
        }
        #endregion
        #region Sample data methods
        private static List<Department> AddSampleDepartments(GirafDbContext context, List<SampleDepartment> sampleDeps)
        {
            string str = new string("Adding departments.");
            Console.WriteLine(str);
            List<Department> departments = new List<Department>();

            foreach (SampleDepartment sampleDepartment in sampleDeps)
            {
                Department department = new Department { Name = sampleDepartment.Name };
                departments.Add(department);
                context.Departments.Add(department);
            }
            context.SaveChanges();
            return departments;
        }

        private static void AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, List<SampleGirafUser> sampleUsers)
        {
            String str2 = new String("Adding users.");
            Console.WriteLine(str2);
            List<GirafUser> users = new List<GirafUser>();
            List<Department> departments = (from dep in context.Departments select dep).ToList();

            foreach (var sampleUser in sampleUsers)
            {
                GirafUser user = new GirafUser
                {
                    UserName = sampleUser.Name,
                    DisplayName = sampleUser.DisplayName,
                    DepartmentKey = departments.FirstOrDefault(d => d.Name == sampleUser.DepartmentName).Key
                };

                var x = userManager.CreateAsync(user, sampleUser.Password).Result.Succeeded;
                if(x)
                {
                    var a = userManager.AddToRoleAsync(user, sampleUser.Role).Result.Succeeded;
                    if (!a)
                    {
                        throw new Exception("Failed to add role " + sampleUser.Role + " to user " + user.UserName);
                    }
                }
                else
                {
                    throw new WarningException("Failed to create user " + user.UserName + " in usermanager");
                }
                users.Add(user);
            }
        }

        private static List<Pictogram> AddSamplePictograms(GirafDbContext context, List<SamplePictogram> samplePictogramsList)
        {
             String str3 = new String("Adding pictograms.");
            System.Console.WriteLine(str3);
            List<Pictogram> pictograms = new List<Pictogram>();
            foreach (var samplePict in samplePictogramsList)
            {
                var pictogram = new Pictogram(samplePict.Title, (AccessLevel)Enum.Parse(typeof(AccessLevel), samplePict.AccessLevel), samplePict.ImageHash);
                pictogram.LastEdit = DateTime.Now;
                context.Add(pictogram);
                pictograms.Add(pictogram);
            }
            return pictograms;
        }

        private static void AddSampleWeekAndWeekdays(GirafDbContext context, List<SampleWeek> sampleWeeks, List<SampleWeekday> sampleWeekdays, List<SampleGirafUser> sampleUsers, List<Pictogram> pictograms)
        {
            String str4 = new string("Adding weekdays to users");
            Console.WriteLine(str4);
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
                AddDaysToWeekAndContext(sampleWeekdays, week, context, pictograms);
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
            List<SampleGirafUser> sampleUsers, List<Department> departments, List<Pictogram> pictograms)
        {
            String str5 = new string("Adding templates");
            Console.WriteLine(str5);
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

                AddDaysToWeekAndContext(sampleWeekdays, template, context, pictograms);
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


        private static void AddDaysToWeekAndContext(List<SampleWeekday> sampleDays, WeekBase week, GirafDbContext context, List<Pictogram> pictograms)
        {
            foreach (var sampleDay in sampleDays)
            {
                Day day = sampleDay.Day;
                List<Pictogram> picts = new List<Pictogram>();
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
                    picts.Add(pic);
                }

                List<ActivityState> activityStates = (from activityState in sampleDay.ActivityStates select (ActivityState)Enum.Parse(typeof(ActivityState), activityState)).ToList<ActivityState>();
                Weekday weekDay = new Weekday(day, picts, activityStates);
                context.Weekdays.Add(weekDay);
                week.UpdateDay(weekDay);
            }
        }
        #endregion
    }
}
