using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
		public async static void Initialize(GirafDbContext context, UserManager<GirafUser> userManager)
		{
			context.Database.EnsureCreated();
			
			if (context.Users.Any())
				return;

			System.Console.WriteLine("Adding some sample data to the database.");
			System.Console.WriteLine("Adding roles.");

			var Roles = new IdentityRole[]
			{
				new IdentityRole("Admin"), 
            	new IdentityRole("Guardian"), 
           		new IdentityRole("User")
			};
			foreach(var role in Roles)
			{
				context.Roles.Add(role);
			}
			context.SaveChanges();

			System.Console.WriteLine("Adding departments.");
			var Departments = new Department[]
			{
				new Department { Name = "Tobias' stue for godt humør"},
				new Department { Name = "Bajer plejen"}
			};
			foreach(var department in Departments)
			{
				context.Departments.Add(department);
			}
			context.SaveChanges();

			System.Console.WriteLine("Adding users.");
			var users = new GirafUser[]
			{
				new GirafUser("Kurt", 1),
				new GirafUser("Graatand", 1),
				new GirafUser("Lee", 2)
			};
			foreach(var user in users)
			{
				await userManager.CreateAsync(user, "password");
			}

			System.Console.WriteLine("Adding pictograms.");
			var Pictograms = new Pictogram[]
			{
				new Pictogram("Hat", AccessLevel.PROTECTED),
				new Pictogram("Kat", AccessLevel.PROTECTED),
				new Pictogram("Nat", AccessLevel.PROTECTED),
				new Pictogram("Slat", AccessLevel.PROTECTED),
				new Pictogram("Pjat", AccessLevel.PROTECTED),
				new Pictogram("Skråt", AccessLevel.PROTECTED),
				new Pictogram("Snot", AccessLevel.PROTECTED),
				new Pictogram("Flot", AccessLevel.PROTECTED),
				new Pictogram("Slot", AccessLevel.PROTECTED),
				new Pictogram("Bil", AccessLevel.PROTECTED),
				new Pictogram("Smil", AccessLevel.PROTECTED),
				new Pictogram("Fil", AccessLevel.PROTECTED),
				new Pictogram("Stil", AccessLevel.PROTECTED),
				new Pictogram("Harald Blåtand", AccessLevel.PUBLIC)
			};
			foreach (var pictogram in Pictograms)
			{
				pictogram.LastEdit = DateTime.Now;
				context.Add(pictogram);
			}
			context.SaveChanges();
			System.Console.WriteLine("Adding pictograms to users.");
			var usr = context.Users.Where(user => user.UserName == "Kurt").First();
			var pictos = new List<Pictogram> { Pictograms[0], Pictograms[1], Pictograms[2], Pictograms[3], Pictograms[4] };
			foreach (var pict in pictos) new UserResource(usr, pict);
			context.SaveChanges();
			usr = context.Users.Where(user => user.UserName == "Lee").First();
			pictos = new List<Pictogram> { Pictograms[5], Pictograms[6], Pictograms[7], Pictograms[8] };
			foreach (var pict in pictos) new UserResource(usr, pict);
			context.SaveChanges();
			usr = context.Users.Where(user => user.UserName == "Graatand").First();
			pictos = new List<Pictogram> { Pictograms[9], Pictograms[10], Pictograms[11], Pictograms[12], Pictograms[13] };
			foreach (var pict in pictos) new UserResource(usr, pict);
			context.SaveChanges();
			
			System.Console.WriteLine("Adding weekdays to users");
			var Weekdays = new Weekday[]
			{
				new Weekday(Days.Monday, Pictograms.Where(p => p.Title == "Hat").First(), 
							new List<Frame> { Pictograms[0], Pictograms[1], Pictograms[2], Pictograms[3], Pictograms[4] }),
				new Weekday(Days.Tuesday, Pictograms.Where(p => p.Title == "Snot").First(), 
							new List<Frame> { Pictograms[5], Pictograms[6], Pictograms[7], Pictograms[8] }),
				new Weekday(Days.Thursday, Pictograms.Where(p => p.Title == "Snot").First(), 
							new List<Frame> { Pictograms[8], Pictograms[6], Pictograms[7], Pictograms[5] }),
				new Weekday(Days.Saturday, Pictograms.Where(p => p.Title == "Snot").First(), 
							new List<Frame> { Pictograms[8], Pictograms[5] }),
				new Weekday(Days.Wednesday,  Pictograms.Where(p => p.Title == "Bil").First(),  
							new List<Frame> { Pictograms[9], Pictograms[10], Pictograms[11], Pictograms[12], Pictograms[13] })
			};

			foreach(var day in Weekdays)
			{
				day.LastEdit = DateTime.Now;
			}
			
			usr = context.Users.Where(u => u.UserName == "Kurt").First();
			usr.WeekSchedule.First().Weekdays.Remove(usr.WeekSchedule.First().Weekdays.Where(wd => wd.Day == Weekdays[0].Day).First());
			usr.WeekSchedule.First().Weekdays.Add(Weekdays[0]);
			usr = context.Users.Where(u => u.UserName == "Lee").First();
			usr.WeekSchedule.First().Weekdays.Remove(usr.WeekSchedule.First().Weekdays.Where(wd => wd.Day == Weekdays[1].Day).First());
			usr.WeekSchedule.First().Weekdays.Add(Weekdays[1]);
			usr.WeekSchedule.First().Weekdays.Remove(usr.WeekSchedule.First().Weekdays.Where(wd => wd.Day == Weekdays[2].Day).First());
			usr.WeekSchedule.First().Weekdays.Add(Weekdays[2]);
			usr.WeekSchedule.First().Weekdays.Remove(usr.WeekSchedule.First().Weekdays.Where(wd => wd.Day == Weekdays[3].Day).First());
			usr.WeekSchedule.First().Weekdays.Add(Weekdays[3]);
			usr = context.Users.Where(u => u.UserName == "Graatand").First();
			usr.WeekSchedule.First().Weekdays.Remove(usr.WeekSchedule.First().Weekdays.Where(wd => wd.Day == Weekdays[4].Day).First());
			usr.WeekSchedule.First().Weekdays.Add(Weekdays[4]);
			context.SaveChanges();

			//Add one of Graatands pictograms to department 1 to see if pictograms are fetched properly.
			new DepartmentResource(Departments[0], Pictograms[9]);
			context.SaveChanges();
		}
    }
}
