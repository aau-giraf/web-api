using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GirafRest.Data.Samples;
using SkiaSharp;

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
        /// <param name="environmentName">The environment set for the current run</param>
        public static async Task Initialize(GirafDbContext context, UserManager<GirafUser> userManager, int pictogramCount, string environmentName)
        {
            // Initialize static fields
            _context = context;
            _userManager = userManager;

            // Verify that the database has already been created, before adding data to it
            _context.Database.EnsureCreated();
            
            //If setting is not present in switch, development setting is chosen as default
            switch (environmentName)
            {
                case "Development":
                    _sampleDataHandler = new SampleDataHandler("DB_data.dev.json");
                    break;

                case "Staging":
                    _sampleDataHandler = new SampleDataHandler("DB_data.stag.json");
                    break;

                case "Production":
                    _sampleDataHandler = new SampleDataHandler("DB_data.prod.json");
                    break;

                default:
                    _sampleDataHandler = new SampleDataHandler("DB_data.dev.json");
                    break;
            }

            // Get sample data
            SampleData sampleData = _sampleDataHandler.DeserializeData();

            // Create departments if they do not exist
            if (!(await _context.Departments.AnyAsync()))
                await AddSampleDepartments(sampleData.DepartmentList);

            if(!(await _context.Roles.AnyAsync()))
            {
                List<string> roles = _sampleDataHandler.ReadSampleRoles(); 
                await AddSampleRoles(roles);
            }
            
            // Create users if they do not exist
            if (!(await _context.Users.AnyAsync())) {
                await AddSampleUsers(sampleData.UserList);
                // Adding citizens to a Guardian
                foreach (var user in _context.Users)
                {
                    if (_userManager.IsInRoleAsync(user, GirafRole.Guardian).Result || _userManager.IsInRoleAsync(user, GirafRole.Trustee).Result)
                    {
                        var citizens = user.Department.Members.Where(m => _userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
                        user.AddCitizens(citizens);
                    }
                }
                await _context.SaveChangesAsync();
            }                

            // Create pictograms if they do not exist
            if (!(await _context.Pictograms.AnyAsync())) {
                List<SamplePictogram> pictograms = _sampleDataHandler.ReadSamplePictograms();
                CreatePictograms(pictogramCount);
                await AddSamplePictograms(pictograms);
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
            Console.WriteLine($"Creating {count} pictograms");
            Directory.CreateDirectory("../pictograms");

            using (var typeface = SKTypeface.Default)
            using (var paint = new SKPaint())
            {
                paint.Typeface = typeface;
                paint.TextSize = 48;
                paint.IsAntialias = true;
                paint.Color = SKColors.Black;

                for (int i = 1; i <= count; i++)
                {
                    SKImage pictogram = DrawText(i.ToString(), paint, SKColors.White);
                    using (Stream stream = File.OpenWrite(Path.Combine("../pictograms", $"{i}.png")))
                    {
                        pictogram.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
                    }
                }
            }
        }

        private static SKImage DrawText(string text, SKPaint paint, SKColor backColor)
        {
            using (var bitmap = new SKBitmap(200, 200))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(backColor);

                // Auto-centers the text
                var textBounds = new SKRect(0,0, 200, 200);
                paint.MeasureText(text, ref textBounds);
                float x = (bitmap.Width - textBounds.Width) / 2;
                float y = (bitmap.Height + textBounds.Height) / 2;

                // Draw the text at the calculated position
                canvas.DrawText(text, x, y, paint);

                return SKImage.FromBitmap(bitmap);
            }
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
        private static async Task AddSampleRoles(List<string> sampleRoles)
        {
            Console.WriteLine("Adding roles...");
            foreach (string sampleRole in sampleRoles)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES ('" + sampleRole + "', '" + Guid.NewGuid().ToString() + "', '" + sampleRole + "', '" + sampleRole.ToUpper() + "');");
                await _context.SaveChangesAsync();
            }
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
            List<Pictogram> pictograms = await _context.Pictograms.ToListAsync();

            foreach (SampleWeek sampleWeek in sampleWeeks)
            {
                Pictogram thumbNail = pictograms.FirstOrDefault(p => p.Title == sampleWeek.ImageTitle);

                Week week = new Week { Name = sampleWeek.Name, Thumbnail = thumbNail };
                await AddDaysToWeekAndContext(sampleWeekdays, week, pictograms);
                await _context.Weeks.AddAsync(week);

                foreach (GirafUser user in _context.Users)
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if ((userWeek == week.Name) && (user.UserName == sampleUser.Name))
                            {
                                user.WeekSchedule.Add(week);
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
        }

        #endregion

        #endregion
    }
}
