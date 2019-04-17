using System;
using System.Collections.Generic;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using static GirafRest.Models.ActivityState;

namespace GirafRest.Setup
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    public class DBInitializer
    {
		public static void Initialize(GirafDbContext context, UserManager<GirafUser> userManager)
		{
            // Check if any data is in the database
            if (context.Users.Any())
				return;

            var departments = AddSampleDepartments(context);
            var users = AddSampleUsers(context, userManager, departments);
            var pictograms = AddSamplePictograms(context);
            AddSampleWeekAndWeekdays(context, pictograms);
            AddSampleWeekTemplate(context, pictograms, departments);
            context.SaveChanges();
			
			//Adding citizens to Guardian
			foreach(var user in context.Users)
			{
                if(userManager.IsInRoleAsync(user, GirafRole.Guardian).Result){
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
        private static IList<Department> AddSampleDepartments(GirafDbContext context)
        {
            Console.WriteLine("Adding some sample data to the database.");
            Console.WriteLine("Adding departments.");
            var departments = new Department[]
            {
                new Department { Name = "Tobias' stue for godt humør"},
                new Department { Name = "Bajer plejen"}
            };
            foreach (var department in departments)
            {
                context.Departments.Add(department);
            }
            context.SaveChanges();

            return departments;
        } 

        private static IList<GirafUser> AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, IList<Department> departments)
        {
            Console.WriteLine("Adding users.");
            GirafUser[] users = new[]
            {
                new GirafUser("Kurt", departments[0]),
                new GirafUser("Graatand", departments[0]),
                new GirafUser{UserName = "Lee"},
                new GirafUser("Tobias", departments[0]),
                new GirafUser("Decker", departments[0])
            };

            //users[0].UserIcon = null;
            //Note that the call to .Result is a dangerous way to run async methods synchonously and thus should not be used elsewhere!
            foreach (var user in users)
            {
                var x = userManager.CreateAsync(user, "password").Result;
            }

            // Add users to roles
            var a = userManager.AddToRoleAsync(users[0], GirafRole.Citizen).Result;
            a = userManager.AddToRoleAsync(users[1], GirafRole.Guardian).Result;
            a = userManager.AddToRoleAsync(users[2], GirafRole.SuperUser).Result;
            a = userManager.AddToRoleAsync(users[3], GirafRole.Department).Result;
            a = userManager.AddToRoleAsync(users[4], GirafRole.Citizen).Result;

            return users;
        }
        #region base64
        private static IList<Pictogram> AddSamplePictograms(GirafDbContext context)
        {
            System.Console.WriteLine("Adding pictograms.");
            var pictograms = new Pictogram[]
            {
                new Pictogram("Epik",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("alfabet",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("alle",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("alting",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("antal",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("berøre",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("bogstav",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("delmængde",      AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("division",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("en",             AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("fantastisk",     AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("farve",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("fem",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("femininum",      AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("figurer",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("fire",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("former",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("fra",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("fredsdue",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("frihed",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("færdig",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("geometriske",    AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("godt",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("grå",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("gul",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("gylden",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("heldig",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("hunkøn",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("hvid",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("hvilken",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("hørelse",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("intet",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("j",              AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("komme",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("kvinde",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("langt",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("lilla",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("line",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("lysegrøn",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("lægge",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("maskulin",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("med",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("mere arbejde",   AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("midterste",      AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("mindre arbejde", AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("multiplikation", AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("mørkeblå",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("mørkegrøn",      AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("nederste",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("nnummer",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("nul",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("numer",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("nuværende",      AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("nærme",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("også",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("ok",             AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("omdømme",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("orange",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("ovenfor",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("parantes",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("parentes",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("pege",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("pink",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("q",              AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("regnestykke",    AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("regnestykke1",   AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("respekt",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("rød",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sammen",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sejt",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("selv",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sig",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("simpelt",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sjovt",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("slut",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("som",            AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sort",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("starte",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("stjerne",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("større end",     AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("symbol",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("synssans",       AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sætte",          AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("sølv",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("tegn",           AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("terning",        AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("tiders",         AccessLevel.PUBLIC, "secure hash"),
                new Pictogram("cat0", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat1", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat2", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat3", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat4", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat5", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat6", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat7", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat8", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat9", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat10", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat11", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat12", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat13", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat14", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat15", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat16", AccessLevel.PUBLIC,"secure hash"),
                new Pictogram("cat17", AccessLevel.PUBLIC,"secure hash"),
            };
            foreach (var pictogram in pictograms)
            {
                pictogram.LastEdit = DateTime.Now;
                context.Add(pictogram);
            }
            context.SaveChanges();

            return pictograms;
        }
        #endregion

        private static void AddSampleWeekAndWeekdays(GirafDbContext context, IList<Pictogram> pictograms)
        {
            Console.WriteLine("Adding weekdays to users");
            var weekdays = new Weekday[]
            {
                new Weekday(Days.Monday, 
                    new List<Pictogram> { pictograms[0], pictograms[1], pictograms[2], pictograms[3], pictograms[4] },
                    new List<ActivityState>{Completed, Completed, Completed, Completed, Completed}),
                
                new Weekday(Days.Tuesday, 
                    new List<Pictogram> { pictograms[5], pictograms[6], pictograms[7], pictograms[8] },
                    new List<ActivityState>{Completed, Active, Canceled, Completed}),
                
                new Weekday(Days.Wednesday, 
                    new List<Pictogram> { pictograms[9], pictograms[10], pictograms[11], pictograms[12], pictograms[13] },
                    new List<ActivityState>{Completed, Active, Active, Active, Active}),
                
                new Weekday(Days.Thursday, 
                    new List<Pictogram> { pictograms[8], pictograms[6], pictograms[7], pictograms[5] },
                    new List<ActivityState>{Active, Canceled, Active, Active}),
                
                new Weekday(Days.Friday, 
                    new List<Pictogram> { pictograms[0], pictograms[7]},
                    new List<ActivityState>{Active, Active}),
                
                new Weekday(Days.Saturday, 
                    new List<Pictogram> { pictograms[8], pictograms[5]},
                    new List<ActivityState>{Canceled, Canceled}),
                
                new Weekday(Days.Sunday, 
                    new List<Pictogram> { pictograms[3], pictograms[5]},
                    new List<ActivityState>{Active, Active})
            };

            var sampleWeek = new Week(pictograms[0]);
            foreach (var day in weekdays)
            {
                context.Weekdays.Add(day);
                sampleWeek.UpdateDay(day);
            }
            sampleWeek.Name = "Normal Uge";
            var usr = context.Users.First(u => u.UserName == "Kurt");
            context.Weeks.Add(sampleWeek);
            usr.WeekSchedule.Add(sampleWeek);
            context.SaveChanges();
        }

        private static void AddSampleWeekTemplate(GirafDbContext context, IList<Pictogram> pictograms, IList<Department> departments)
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
