using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafEntities.Samples;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRepositories.User;
using GirafServices.User;
using GirafServices.WeekPlanner;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SkiaSharp;

namespace GirafAPI.Setup
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    public class DBInitializer
    {
        private  SampleDataHandler _sampleDataHandler { get; set; }
        private  UserManager<GirafUser> _userManager { get; set; }

        private readonly IUserService _userService;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IGirafUserRepository _userRepository;
        private readonly IWeekBaseService _weekBaseService;
        #region Methods

        public DBInitializer(IUserService userService, IDatabaseRepository databaseRepository, IGirafUserRepository userRepository, IWeekBaseService weekBaseService)
        {
            _userService = userService;
            _databaseRepository = databaseRepository;
            _userRepository = userRepository;
            _weekBaseService = weekBaseService;
        }

        /// <summary>
        /// Runs specificed initialisations before the API is started.
        /// </summary>
        /// <param name="context">The database _context.</param>
        /// <param name="userManager">The API for managing GirafUsers.</param>
        /// <param name="pictogramCount">The number of sample pictograms to generate.</param>
        /// <param name="environmentName">The environment set for the current run</param>
        public async Task Initialize(GirafDbContext context, UserManager<GirafUser> userManager, int pictogramCount, string environmentName)
        {
            // Initialize static fields
            _userManager = userManager;
            ArgumentNullException.ThrowIfNull(_userManager);

            // Verify that the database has already been created, before adding data to it
            _databaseRepository.EnsureCreated();


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
                    break;
            }

            // Get sample data
            SampleData sampleData = _sampleDataHandler.DeserializeData();

            // Create departments if they do not exist
            if (! await _databaseRepository.AnyAsync<Department>())
                await AddSampleDepartments(sampleData.DepartmentList);

            if (!await _databaseRepository.AnyAsync<GirafRole>())
            {
                List<string> roles = _sampleDataHandler.ReadSampleRoles();
                await AddSampleRoles(roles);
            }

            // Create users if they do not exist
            if (!await _databaseRepository.AnyAsync<GirafUser>())
            {
                await AddSampleUsers(sampleData.UserList);
                // Adding citizens to a Guardian
                foreach (var user in _userRepository.GetAll())
                {
                    if (_userManager.IsInRoleAsync(user, GirafRole.Guardian).Result || _userManager.IsInRoleAsync(user, GirafRole.Trustee).Result)
                    {
                        var citizens = user.Department.Members.Where(m => _userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
                        foreach (var citizen in citizens)
                        {
                            _userService.AddCitizen(citizen, user);
                        }
                    }
                }
                await _databaseRepository.SaveChangesAsync();
            }

            // Create pictograms if they do not exist
            if (!await _databaseRepository.AnyAsync<Pictogram>())
            {
                List<SamplePictogram> pictograms = _sampleDataHandler.ReadSamplePictograms();
                CreatePictograms(pictogramCount);
                await AddSamplePictograms(pictograms);
            }

            // Create week and weekdays if they do not exist
            if (!await _databaseRepository.AnyAsync<Week>())
                await AddSampleWeekAndWeekdays(sampleData.WeekList, sampleData.WeekdayList, sampleData.UserList);

            // Create week templates if they do not exist
            if (!await _databaseRepository.AnyAsync<WeekTemplate>())
                await AddSampleWeekTemplate(sampleData.WeekTemplateList, sampleData.WeekdayList, sampleData.UserList);


            // Save changes
            await _databaseRepository.SaveChangesAsync();
        }

        #region Generating pictograms
        //Method for generating pictograms
        private void CreatePictograms(int count)
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

        private SKImage DrawText(string text, SKPaint paint, SKColor backColor)
        {
            using (var bitmap = new SKBitmap(200, 200))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(backColor);

                // Auto-centers the text
                var textBounds = new SKRect(0, 0, 200, 200);
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
        private void AddPictogramsToUser(GirafUser user, params Pictogram[] pictograms)
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

        private void AddPictogramToDepartment(Department department, params Pictogram[] pictograms)
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

        private async Task AddSampleDepartments(List<SampleDepartment> sampleDeps)
        {
            Console.WriteLine("Adding departments.");

            foreach (SampleDepartment sampleDepartment in sampleDeps)
            {
                Department department = new Department { Name = sampleDepartment.Name };
                await _databaseRepository.AddAsync<Department>(department);
            }
            await _databaseRepository.SaveChangesAsync();
        }
        private async Task AddSampleRoles(List<string> sampleRoles)
        {
            Console.WriteLine("Adding roles...");
            foreach (string sampleRole in sampleRoles)
            {
                await _databaseRepository.ExecuteSqlRawAsync(
                    "INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName) VALUES ('" + sampleRole + "', '" + Guid.NewGuid().ToString() + "', '" + sampleRole + "', '" + sampleRole.ToUpper() + "');");
                await _databaseRepository.SaveChangesAsync();
            }
        }
        private async Task AddSampleUsers(List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding users.");
            List<Department> departments = await _databaseRepository.ToListAsync<Department>();

            foreach (var sampleUser in sampleUsers)
            {
                GirafUser user = new GirafUser
                {
                    UserName = sampleUser.Name,
                    DisplayName = sampleUser.DisplayName,
                    DepartmentKey = departments.FirstOrDefault(d => d.Name == sampleUser.DepartmentName).Key
                };

                var x = await _userManager.CreateAsync(user, sampleUser.Password);
                await _databaseRepository.SaveChangesAsync();

                if (x.Succeeded)
                {
                    var a = await _userManager.AddToRoleAsync(user, sampleUser.Role);
                    await _databaseRepository.SaveChangesAsync();
                    if (!a.Succeeded)
                        throw new Exception("Failed to add role " + sampleUser.Role + " to user " + user.UserName);
                }
                else
                    throw new WarningException("Failed to create user " + user.UserName + " in usermanager");
            }
        }
        private async Task<List<Pictogram>> AddSamplePictograms(List<SamplePictogram> samplePictogramsList)
        {
            System.Console.WriteLine("Adding pictograms.");
            List<Pictogram> pictograms = new List<Pictogram>();
            foreach (var samplePict in samplePictogramsList)
            {
                Pictogram pictogram = new Pictogram
                {
                    Title = samplePict.Title,
                    AccessLevel = (AccessLevel)Enum.Parse(typeof(AccessLevel), samplePict.AccessLevel),
                    ImageHash = samplePict.ImageHash,
                    LastEdit = DateTime.Now
                };
                await _databaseRepository.AddAsync<Pictogram>(pictogram);
                pictograms.Add(pictogram);
            }
            await _databaseRepository.SaveChangesAsync();
            return pictograms;
        }
        private async Task AddSampleWeekAndWeekdays(List<SampleWeek> sampleWeeks, List<SampleWeekday> sampleWeekdays, List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding weekdays to users");
            List<Pictogram> pictograms = await _databaseRepository.ToListAsync<Pictogram>();

            foreach (SampleWeek sampleWeek in sampleWeeks)
            {
                Pictogram thumbNail = pictograms.FirstOrDefault(p => p.Title == sampleWeek.ImageTitle);

                Week week = new Week { Name = sampleWeek.Name, Thumbnail = thumbNail };
                await AddDaysToWeekAndContext(sampleWeekdays, week, pictograms);
                await _databaseRepository.AddAsync<Week>(week);

                foreach (GirafUser user in _userRepository.GetAll())
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if (userWeek == week.Name && user.UserName == sampleUser.Name)
                            {
                                user.WeekSchedule.Add(week);
                            }
                        }
                    }
                }
            }
            await _databaseRepository.SaveChangesAsync();
        }
        private async Task AddSampleWeekTemplate(
            List<SampleWeekTemplate> sampleTemplates,
            List<SampleWeekday> sampleWeekdays,
            List<SampleGirafUser> sampleUsers)
        {
            Console.WriteLine("Adding templates");
            List<Department> departments = await _databaseRepository.ToListAsync<Department>();
            List<Pictogram> pictograms = await _databaseRepository.ToListAsync<Pictogram>();

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
                await _databaseRepository.AddAsync<WeekTemplate>(template);

                foreach (GirafUser user in _userRepository.GetAll())
                {
                    foreach (SampleGirafUser sampleUser in sampleUsers)
                    {
                        foreach (string userWeek in sampleUser.Weeks)
                        {
                            if (userWeek == template.Name && user.UserName == sampleUser.Name)
                            {
                                user.WeekSchedule.Add(await _databaseRepository.FirstAsync<Week>(w => w.Name == userWeek));
                            }
                        }
                    }
                }
            }
            await _databaseRepository.SaveChangesAsync();
        }
        private async Task AddDaysToWeekAndContext(List<SampleWeekday> sampleDays, WeekBase week, List<Pictogram> pictograms)
        {
            foreach (var sampleDay in sampleDays)
            {
                Days day = sampleDay.Day;
                List<List<Pictogram>> picts = new List<List<Pictogram>>();

                foreach (string actIcon in sampleDay.ActivityIconTitles)
                {
                    Pictogram pic = pictograms.FirstOrDefault(p => p.Title == actIcon);
                    picts.Add(new List<Pictogram> { pic });
                }

                List<ActivityState> activityStates = (from activityState in sampleDay.ActivityStates select (ActivityState)Enum.Parse(typeof(ActivityState), activityState)).ToList<ActivityState>();
                Weekday weekDay = new Weekday(day, picts, activityStates);
                await _databaseRepository.AddAsync<Weekday>(weekDay);
                _weekBaseService.UpdateDay(weekDay, week);
            }
        }

        #endregion

        #endregion
    }
}
