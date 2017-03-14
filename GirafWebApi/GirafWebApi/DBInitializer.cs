using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafWebApi
{
    public class DBInitializer
    {
		public static void Initialize(GirafDbContext context)
		{
			context.Database.EnsureCreated();
			if (context.Users.Any())
				return;
			
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

			var Departments = new Department[]
			{
				new Department { Name = "Tobias' stue for godt humør", Key = 1},
				new Department { Name = "Bajer plejen", Key = 2}
			};
			
			foreach(var department in Departments)
			{
				context.Departments.Add(department);
			}
			context.SaveChanges();

			var users = new GirafUser[]
			{
				new GirafUser("Kurt", "password", Roles.Where(role => role.Name == "User").First()) { Department_Key = 1},
				new GirafUser("Harald Gråtand", "password", Roles.Where(role => role.Name == "Guardian").First()) {Department_Key = 1},
				new GirafUser("Lee", "password", Roles.Where(role => role.Name == "Admin").First()) { Department_Key = 2}
			};
			
			foreach(var user in users)
			{
				context.Departments.Where(dep => dep.Key == user.Department_Key).First().members.Add(user);
				context.Users.Add(user);
			}
			context.SaveChanges();
			
			var Pictograms = new Pictogram[]
			{
				new Pictogram("Hat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Kurt").First().Id },
				new Pictogram("Kat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Kurt").First().Id },
				new Pictogram("Nat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Kurt").First().Id },
				new Pictogram("Slat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Kurt").First().Id },
				new Pictogram("Pjat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Kurt").First().Id },
				new Pictogram("Skråt", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Lee").First().Id },
				new Pictogram("Snot", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Lee").First().Id },
				new Pictogram("Flot", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Lee").First().Id },
				new Pictogram("Slot", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Lee").First().Id },
				new Pictogram("Bil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Harald Gråtand").First().Id },
				new Pictogram("Smil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Harald Gråtand").First().Id },
				new Pictogram("Fil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Harald Gråtand").First().Id },
				new Pictogram("Stil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Harald Gråtand").First().Id },
				new Pictogram("Harald Blåtand", AccessLevel.PUBLIC, new GirafImage()) { owner_id = users.Where(usr => usr.UserName == "Harald Gråtand").First().Id }
			};

			foreach (var pictogram in Pictograms)
			{
				context.Add(pictogram);
			}
			context.SaveChanges();
		}
    }
}
