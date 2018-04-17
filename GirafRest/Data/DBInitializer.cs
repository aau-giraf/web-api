using System;
using System.Collections.Generic;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace GirafRest.Setup
{
    /// <summary>
    /// A class for initializing the database with some sample data.
    /// </summary>
    public class DBInitializer
    {
		/// <summary>
		/// Initializes the local database with sample data.
		/// </summary>
		/// <param name="context">A reference to the database context.</param>
		/// <param name="userManager">A reference to the userManager</param>
		public static void Initialize(GirafDbContext context, UserManager<GirafUser> userManager)
		{
            // Check if any data is in the database
            if (context.Users.Any())
				return;

            var Departments = AddSampleDepartments(context);
            var Users = AddSampleUsers(context, userManager, Departments);
            var Pictograms = AddSamplePictograms(context);
            var choices = AddSampleChoices(context, Pictograms);
            AddSampleWeekAndWeekdays(context, Pictograms, choices);
            AddSampleWeekTemplate(context, Pictograms, choices, Departments);
            context.SaveChanges();
            // //For simplicity we simply add all the private pictograms to all users.
            // foreach (var usr in context.Users)
            // {
            //     addPictogramsToUser(usr, Pictograms[0], Pictograms[1], Pictograms[2]);
            // }
			// context.SaveChanges();

            // //Each department owns a single pictogram.
            // addPictogramToDepartment(Departments[0], Pictograms[3]);
            // addPictogramToDepartment(Departments[1], Pictograms[4]);
			// context.SaveChanges();
			
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
        private static void addPictogramsToUser(GirafUser user, params Pictogram[] pictograms)
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

        private static void addPictogramToDepartment(Department department, params Pictogram[] pictograms)
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
            System.Console.WriteLine("Adding some sample data to the database.");
            System.Console.WriteLine("Adding departments.");
            var Departments = new Department[]
            {
                new Department { Name = "Tobias' stue for godt humør"},
                new Department { Name = "Bajer plejen"}
            };
            foreach (var department in Departments)
            {
                context.Departments.Add(department);
            }
            context.SaveChanges();

            return Departments;
        } 

        private static IList<GirafUser> AddSampleUsers(GirafDbContext context, UserManager<GirafUser> userManager, IList<Department> Departments)
        {
            System.Console.WriteLine("Adding users.");
            var users = new GirafUser[]
            {
                new GirafUser("Kurt", Departments[0]),
                new GirafUser("Graatand", Departments[0]),
                new GirafUser("Lee", Departments[1]),
                new GirafUser("Tobias", Departments[0]),
                new GirafUser("Decker", Departments[0])
            };
            users[0].UserIcon = Encoding.ASCII.GetBytes(HugeBase64Images.Image1);
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
            var Pictograms = new Pictogram[]
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
            foreach (var pictogram in Pictograms)
            {
                pictogram.LastEdit = DateTime.Now;
                context.Add(pictogram);
            }
            context.SaveChanges();

            return Pictograms;
        }
        #endregion
        private static IList<Choice> AddSampleChoices(GirafDbContext context, IList<Pictogram> Pictograms)
        {
            var choices = new List<Choice>();

            choices.Add(createChoice(Pictograms[13]));
            choices.Add(createChoice(Pictograms[0], Pictograms[4]));
            choices.Add(createChoice(Pictograms[5], Pictograms[8]));

            foreach (var choice in choices)
            {
                context.Choices.Add(choice);
            }
            context.SaveChanges();
            return choices;
        }
        private static Choice createChoice(params Pictogram[] pictograms)
        {
            return new Choice(pictograms.ToList(), "Choice" + pictograms.Length);
        }

        private static void AddSampleWeekAndWeekdays(GirafDbContext context, IList<Pictogram> Pictograms, 
            IList<Choice> Choices)
        {
            Console.WriteLine("Adding weekdays to users");
            var Weekdays = new Weekday[]
            {
                new Weekday(Days.Monday, new List<Resource> { Pictograms[0], Pictograms[1], Pictograms[2], Pictograms[3], Pictograms[4] }),
                new Weekday(Days.Tuesday, new List<Resource> { Pictograms[5], Pictograms[6], Pictograms[7], Pictograms[8] }),
                new Weekday(Days.Wednesday, new List<Resource> { Pictograms[9], Pictograms[10], Pictograms[11], Pictograms[12], Pictograms[13] }),
                new Weekday(Days.Thursday, new List<Resource> { Pictograms[8], Pictograms[6], Pictograms[7], Pictograms[5] }),
                new Weekday(Days.Friday, new List<Resource> { Pictograms[0], Pictograms[7]}),
                new Weekday(Days.Saturday, new List<Resource> { Pictograms[8], Pictograms[5], Choices[1] }),
                new Weekday(Days.Sunday, new List<Resource> { Pictograms[3], Pictograms[5], Choices[0] })
            };

            var sampleWeek = new Week(Pictograms[0]);
            foreach (var day in Weekdays)
            {
                day.LastEdit = DateTime.Now;
                context.Weekdays.Add(day);
                sampleWeek.UpdateDay(day);
            }
            sampleWeek.Name = "Normal Uge";
            var usr = context.Users.Where(u => u.UserName == "Kurt").First();
            context.Weeks.Add(sampleWeek);
            usr.WeekSchedule.Add(sampleWeek);
            context.SaveChanges();
        }

        private static void AddSampleWeekTemplate(GirafDbContext context, IList<Pictogram> Pictograms,
            IList<Choice> Choices, IList<Department> depertments)
        {
            Console.WriteLine("Adding weekdays to users");
            var Weekdays = new Weekday[]
            {
                new Weekday(Days.Monday, new List<Resource> { Pictograms[0], Pictograms[1], Pictograms[2], Pictograms[3], Pictograms[4] }),
                new Weekday(Days.Tuesday, new List<Resource> { Pictograms[5], Pictograms[6], Pictograms[7], Pictograms[8] }),
                new Weekday(Days.Wednesday, new List<Resource> { Pictograms[9], Pictograms[10], Pictograms[11], Pictograms[12], Pictograms[13] }),
                new Weekday(Days.Thursday, new List<Resource> { Pictograms[8], Pictograms[6], Pictograms[7], Pictograms[5] }),
                new Weekday(Days.Friday, new List<Resource> { Pictograms[0], Pictograms[7]}),
                new Weekday(Days.Saturday, new List<Resource> { Pictograms[8], Pictograms[5], Choices[1] }),
                new Weekday(Days.Sunday, new List<Resource> { Pictograms[3], Pictograms[5], Choices[0] })
            };

            var sampleWeek = new WeekTemplate(Pictograms[0]);
            foreach (var day in Weekdays)
            {
                day.LastEdit = DateTime.Now;
                context.Weekdays.Add(day);
                sampleWeek.UpdateDay(day);
            }

            sampleWeek.Name = "Skabelonuge";
            sampleWeek.DepartmentKey = depertments[0].Key;

            context.WeekTemplates.Add(sampleWeek);
            context.SaveChanges();
        }
        #endregion
    }
}
