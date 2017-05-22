using System;
using System.Collections.Generic;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;

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

            //For simplicity we simply add all the private pictograms to all users.
            foreach (var usr in Users)
            {
                addPictogramsToUser(usr, Pictograms[0], Pictograms[1], Pictograms[2]);
            }
			context.SaveChanges();

            //Each department owns a single pictogram.
            addPictogramToDepartment(Departments[0], Pictograms[3]);
            addPictogramToDepartment(Departments[1], Pictograms[4]);
			context.SaveChanges();
			
			//Adding citizens to Guardian
			foreach(var user in Users)
			{
				if(userManager.IsInRoleAsync(user, GirafRole.Guardian).Result){
					user.GuardianOf = user.Department.Members
					.Where(m => userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
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
                new GirafUser("Tobias", Departments[0])
            };
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

            return users;
        }

        private static IList<Pictogram> AddSamplePictograms(GirafDbContext context)
        {
            System.Console.WriteLine("Adding pictograms.");
            var Pictograms = new Pictogram[]
            {
                new Pictogram("Hat", AccessLevel.PRIVATE),
                new Pictogram("Kat", AccessLevel.PRIVATE),
                new Pictogram("Nat", AccessLevel.PRIVATE),
                new Pictogram("Slat", AccessLevel.PROTECTED),
                new Pictogram("Pjat", AccessLevel.PROTECTED),
                new Pictogram("Skråt", AccessLevel.PUBLIC),
                new Pictogram("Snot", AccessLevel.PUBLIC),
                new Pictogram("Flot", AccessLevel.PUBLIC),
                new Pictogram("Slot", AccessLevel.PUBLIC),
                new Pictogram("Bil", AccessLevel.PUBLIC),
                new Pictogram("Smil", AccessLevel.PUBLIC),
                new Pictogram("Fil", AccessLevel.PUBLIC),
                new Pictogram("Stil", AccessLevel.PUBLIC),
                new Pictogram("Harald Blåtand", AccessLevel.PUBLIC)
            };
            foreach (var pictogram in Pictograms)
            {
                pictogram.LastEdit = DateTime.Now;
                context.Add(pictogram);
            }
            context.SaveChanges();

            return Pictograms;
        }

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

            var usr = context.Users.Where(u => u.UserName == "Kurt").First();
            context.Weeks.Add(sampleWeek);
            usr.WeekSchedule.Add(sampleWeek);
            context.SaveChanges();
        }
        #endregion
    }
}
