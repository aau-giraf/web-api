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

            //users[0].UserIcon = Encoding.ASCII.GetBytes(HugeBase64Images.Image1);
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
                new Pictogram("Epik",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image2)),
                new Pictogram("alfabet",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image3)),
                new Pictogram("alle",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image4)),
                new Pictogram("alting",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image5)),
                new Pictogram("antal",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image6)),
                new Pictogram("berøre",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image7)),
                new Pictogram("bogstav",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image8)),
                new Pictogram("delmængde",      AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image9)),
                new Pictogram("division",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image10)),
                new Pictogram("en",             AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image11)),
                new Pictogram("fantastisk",     AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image13)),
                new Pictogram("farve",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image14)),
                new Pictogram("fem",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image15)),
                new Pictogram("femininum",      AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image16)),
                new Pictogram("figurer",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image17)),
                new Pictogram("fire",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image18)),
                new Pictogram("former",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image19)),
                new Pictogram("fra",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image20)),
                new Pictogram("fredsdue",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image21)),
                new Pictogram("frihed",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image22)),
                new Pictogram("færdig",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image23)),
                new Pictogram("geometriske",    AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image24)),
                new Pictogram("godt",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image25)),
                new Pictogram("grå",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image26)),
                new Pictogram("gul",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image27)),
                new Pictogram("gylden",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image29)),
                new Pictogram("heldig",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image30)),
                new Pictogram("hunkøn",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image31)),
                new Pictogram("hvid",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image32)),
                new Pictogram("hvilken",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image33)),
                new Pictogram("hørelse",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image34)),
                new Pictogram("intet",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image35)),
                new Pictogram("j",              AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image36)),
                new Pictogram("komme",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image37)),
                new Pictogram("kvinde",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image38)),
                new Pictogram("langt",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image39)),
                new Pictogram("lilla",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image40)),
                new Pictogram("line",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image41)),
                new Pictogram("lysegrøn",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image42)),
                new Pictogram("lægge",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image43)),
                new Pictogram("maskulin",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image44)),
                new Pictogram("med",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image45)),
                new Pictogram("mere arbejde",   AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image46)),
                new Pictogram("midterste",      AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image47)),
                new Pictogram("mindre arbejde", AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image48)),
                new Pictogram("multiplikation", AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image49)),
                new Pictogram("mørkeblå",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image50)),
                new Pictogram("mørkegrøn",      AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image51)),
                new Pictogram("nederste",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image52)),
                new Pictogram("nnummer",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image53)),
                new Pictogram("nul",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image54)),
                new Pictogram("numer",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image55)),
                new Pictogram("nuværende",      AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image56)),
                new Pictogram("nærme",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image57)),
                new Pictogram("også",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image58)),
                new Pictogram("ok",             AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image59)),
                new Pictogram("omdømme",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image60)),
                new Pictogram("orange",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image61)),
                new Pictogram("ovenfor",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image62)),
                new Pictogram("parantes",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image63)),
                new Pictogram("parentes",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image64)),
                new Pictogram("pege",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image65)),
                new Pictogram("pink",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image66)),
                new Pictogram("q",              AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image67)),
                new Pictogram("regnestykke",    AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image68)),
                new Pictogram("regnestykke1",   AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image69)),
                new Pictogram("respekt",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image70)),
                new Pictogram("rød",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image71)),
                new Pictogram("sammen",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image72)),
                new Pictogram("sejt",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image73)),
                new Pictogram("selv",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image74)),
                new Pictogram("sig",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image75)),
                new Pictogram("simpelt",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image76)),
                new Pictogram("sjovt",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image77)),
                new Pictogram("slut",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image78)),
                new Pictogram("som",            AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image79)),
                new Pictogram("sort",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image80)),
                new Pictogram("starte",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image81)),
                new Pictogram("stjerne",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image82)),
                new Pictogram("større end",     AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image83)),
                new Pictogram("symbol",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image84)),
                new Pictogram("synssans",       AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image85)),
                new Pictogram("sætte",          AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image86)),
                new Pictogram("sølv",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image87)),
                new Pictogram("tegn",           AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image88)),
                new Pictogram("terning",        AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image89)),
                new Pictogram("tiders",         AccessLevel.PUBLIC, Encoding.ASCII.GetBytes(HugeBase64Images.Image90)),
                new Pictogram("cat0", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image91)),
                new Pictogram("cat1", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image92)),
                new Pictogram("cat2", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image93)),
                new Pictogram("cat3", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image94)),
                new Pictogram("cat4", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image95)),
                new Pictogram("cat5", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image96)),
                new Pictogram("cat6", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image97)),
                new Pictogram("cat7", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image98)),
                new Pictogram("cat8", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image99)),
                new Pictogram("cat9", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image100)),
                new Pictogram("cat10", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image101)),
                new Pictogram("cat11", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image102)),
                new Pictogram("cat12", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image103)),
                new Pictogram("cat13", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image104)),
                new Pictogram("cat14", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image105)),
                new Pictogram("cat15", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image106)),
                new Pictogram("cat16", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image107)),
                new Pictogram("cat17", AccessLevel.PUBLIC,Encoding.ASCII.GetBytes(HugeBase64Images.Image108)),
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
