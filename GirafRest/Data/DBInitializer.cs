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
using GirafRest.Data.Samples;

namespace GirafRest.Data
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    public static class DBInitializer
    {
        private static SampleDataHandler _sampleDataHandler { get; set; }
        private static UserManager<GirafUser> _userManager { get; set; }
        private static GirafDbContext _context { get; set; }
        #region Methods

        /// <summary>
        /// Runs specificed initialisations before the API is started.
        /// </summary>
        /// <param name="context">The database _context.</param>
        /// <param name="userManager">The API for managing GirafUsers.</param>
        /// <param name="pictogramCount">The number of sample pictograms to generate.</param>
        public static async Task Initialize(GirafDbContext context, UserManager<GirafUser> userManager, int pictogramCount)
        {
            // Initialize static fields
            _context = context;
            _userManager = userManager;
            _sampleDataHandler = new SampleDataHandler();

            // Verify that the database has already been created, before adding data to it
            _context.Database.EnsureCreated();

            // Get sample data
            SampleData sampleData = _sampleDataHandler.DeserializeData();

            // Create departments if they do not exist
            if (!(await _context.Departments.AnyAsync()))
                await AddSampleDepartments(sampleData.DepartmentList);
            
            // Create users if they do not exist
            if (!(await _context.Users.AnyAsync())) {
                await AddSampleUsers(sampleData.UserList);
                // Adding citizens to a Guardian
                foreach (var user in _context.Users)
                {
                    if (_userManager.IsInRoleAsync(user, GirafRole.Guardian).Result)
                    {
                        var citizens = user.Department.Members.Where(m => _userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
                        user.AddCitizens(citizens);
                    }
                }
                await _context.SaveChangesAsync();
            }                

            // Create pictograms if they do not exist
            if (!(await _context.Pictograms.AnyAsync())) {
                CreatePictograms(pictogramCount);
                await AddSamplePictograms(sampleData.PictogramList);
            }

            // Create week and weekdays if they do not exist
            if (!(await _context.Weeks.AnyAsync()))
                await AddSampleWeekAndWeekdays(sampleData.WeekList, sampleData.WeekdayList, sampleData.UserList);
            
            // Create week templates if they do not exist
            if (!(await _context.WeekTemplates.AnyAsync()))
                await AddSampleWeekTemplate(sampleData.WeekTemplateList, sampleData.WeekdayList, sampleData.UserList);

            
            // Save changes
            await _context.SaveChangesAsync();   
        }

        #region Generating pictograms
        //Method for generating pictograms
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
                using Image pictogram = DrawText(i.ToString(), font, Color.Black, Color.White);
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

        #region Adding sample data

        private static async Task AddSampleDepartments(List<SampleDepartment> sampleDeps)
        {
            Console.WriteLine("Adding departments.");

            foreach (SampleDepartment sampleDepartment in sampleDeps)
            {
                Department department = new Department { Name = sampleDepartment.Name };
                await _context.Departments.AddAsync(department);
            }
            await _context.SaveChangesAsync();
        }
        private static async Task AddSampleUsers(List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding users.");
            List<Department> departments = await _context.Departments.ToListAsync();

            foreach (var sampleUser in sampleUsers)
            {
                GirafUser user = new GirafUser
                {
                    UserName = sampleUser.Name,
                    DisplayName = sampleUser.DisplayName,
                    DepartmentKey = departments.FirstOrDefault(d => d.Name == sampleUser.DepartmentName).Key
                };

                var x = await _userManager.CreateAsync(user, sampleUser.Password);
                await _context.SaveChangesAsync();

                if (x.Succeeded)
                {
                    var a = await _userManager.AddToRoleAsync(user, sampleUser.Role);
                    await _context.SaveChangesAsync();
                    if (!a.Succeeded)
                        throw new Exception("Failed to add role " + sampleUser.Role + " to user " + user.UserName);
                }
                else
                    throw new WarningException("Failed to create user " + user.UserName + " in usermanager");
            }
        }
        private static async Task<List<Pictogram>> AddSamplePictograms(List<SamplePictogram> samplePictogramsList)
        {
            System.Console.WriteLine("Adding pictograms.");
            List<Pictogram> pictograms = new List<Pictogram>();
            foreach (var samplePict in samplePictogramsList)
            {
                Pictogram pictogram = new Pictogram {
                    Title = samplePict.Title, 
                    AccessLevel = (AccessLevel)Enum.Parse(typeof(AccessLevel), samplePict.AccessLevel), 
                    ImageHash = samplePict.ImageHash,
                    LastEdit = DateTime.Now
                };
                await _context.AddAsync(pictogram);
                pictograms.Add(pictogram);
            }
            await _context.SaveChangesAsync();
            return pictograms;
        }
        private static async Task AddSampleWeekAndWeekdays(List<SampleWeek> sampleWeeks, List<SampleWeekday> sampleWeekdays, List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding weekdays to users");
            List<Week> weekList = new List<Week>();
            List<Pictogram> pictograms = await _context.Pictograms.ToListAsync();

            foreach (SampleWeek sampleWeek in sampleWeeks)
            {
                Pictogram thumbNail = pictograms.FirstOrDefault(p => p.Title == sampleWeek.ImageTitle);

                Week week = new Week { Name = sampleWeek.Name, Thumbnail = thumbNail };
                await AddDaysToWeekAndContext(sampleWeekdays, week, pictograms);
                await _context.Weeks.AddAsync(week);
                weekList.Add(week);
                await _context.SaveChangesAsync();

                foreach (GirafUser user in _context.Users)
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
            await _context.SaveChangesAsync();
        }
        private static async Task AddSampleWeekTemplate(
            List<SampleWeekTemplate> sampleTemplates, 
            List<SampleWeekday> sampleWeekdays,
            List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding templates");
            List<Department> departments = await _context.Departments.ToListAsync();
            List<Pictogram> pictograms = await _context.Pictograms.ToListAsync();

            foreach (SampleWeekTemplate sampleTemplate in sampleTemplates)
            {
                Pictogram thumbNail = pictograms.FirstOrDefault(p => p.Title == sampleTemplate.ImageTitle);

                WeekTemplate template = new WeekTemplate
                {
                    Name = sampleTemplate.Name,
                    Thumbnail = thumbNail,
                    DepartmentKey = departments.First(d => d.Name == sampleTemplate.DepartmentName).Key
                };

                await AddDaysToWeekAndContext(sampleWeekdays, template, pictograms);
                await _context.WeekTemplates.AddAsync(template);
                await _context.SaveChangesAsync();

                foreach (GirafUser user in _context.Users)
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if ((userWeek == template.Name) && (user.UserName == sampleUser.Name))
                            {
                                user.WeekSchedule.Add(await _context.Weeks.FirstAsync(w => w.Name == userWeek));
                            }
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
        private static async Task AddDaysToWeekAndContext(List<SampleWeekday> sampleDays, WeekBase week, List<Pictogram> pictograms)
        {
            foreach (var sampleDay in sampleDays)
            {
                Days day = sampleDay.Day;
                List<List<Pictogram>> picts = new List<List<Pictogram>>();

                foreach (string actIcon in sampleDay.ActivityIconTitles)
                {
                    Pictogram pic = pictograms.FirstOrDefault(p => p.Title == actIcon);
                    picts.Add(new List<Pictogram>{ pic });
                }

                List<ActivityState> activityStates = (from activityState in sampleDay.ActivityStates select (ActivityState)Enum.Parse(typeof(ActivityState), activityState)).ToList<ActivityState>();
                Weekday weekDay = new Weekday(day, picts, activityStates);
                await _context.Weekdays.AddAsync(weekDay);
                week.UpdateDay(weekDay);
            }
            await _context.SaveChangesAsync();
        }

        #endregion

        #endregion
    }
}
