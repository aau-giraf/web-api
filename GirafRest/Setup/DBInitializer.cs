using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
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
		public static void Initialize(GirafDbContext context, UserManager<GirafUser> userManager)
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
				new GirafUser("Kurt"),
				new GirafUser("Harald Gråtand"),
				new GirafUser("Lee")
			};
			
			foreach(var user in users)
			{
				userManager.CreateAsync(user, "password");
			}
			context.Departments.Where(dep => dep.Key == 1).First().Members.Add(users[0]);
			context.Departments.Where(dep => dep.Key == 1).First().Members.Add(users[1]);
			context.Departments.Where(dep => dep.Key == 2).First().Members.Add(users[2]);
			context.SaveChanges();

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
				context.Add(pictogram);
			}
			var usr = context.Users.Where(user => user.UserName == "Kurt").First();
			usr.Resources.Add(Pictograms[0]);
			usr.Resources.Add(Pictograms[1]);
			usr.Resources.Add(Pictograms[2]);
			usr.Resources.Add(Pictograms[3]);
			usr.Resources.Add(Pictograms[4]);
			usr = context.Users.Where(user => user.UserName == "Lee").First();
			usr.Resources.Add(Pictograms[5]);
			usr.Resources.Add(Pictograms[6]);
			usr.Resources.Add(Pictograms[7]);
			usr.Resources.Add(Pictograms[8]);
			usr = context.Users.Where(user => user.UserName == "Harald Gråtand").First();
			usr.Resources.Add(Pictograms[9]);
			usr.Resources.Add(Pictograms[10]);
			usr.Resources.Add(Pictograms[11]);
			usr.Resources.Add(Pictograms[12]);
			usr.Resources.Add(Pictograms[13]);
			context.SaveChanges();
		}
    }
}
