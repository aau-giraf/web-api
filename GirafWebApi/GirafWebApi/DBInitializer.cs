using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafWebApi.Contexts;
using GirafWebApi.Models;

namespace GirafWebApi
{
    public class DBInitializer
    {
		public static void Initialize(GirafDbContext context)
		{
			context.Database.EnsureCreated();

			if (context.Users.Any())
				return;
			var Pictograms = new Pictogram[]
			{
				new Pictogram("Hat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 1 },
				new Pictogram("Kat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 1 },
				new Pictogram("Nat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 1 },
				new Pictogram("Slat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 1 },
				new Pictogram("Pjat", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 1 },
				new Pictogram("Skråt", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 2 },
				new Pictogram("Snot", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 2 },
				new Pictogram("Flot", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 2 },
				new Pictogram("Slot", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 2 },
				new Pictogram("Bil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 3 },
				new Pictogram("Smil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 3 },
				new Pictogram("Fil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 3 },
				new Pictogram("Stil", AccessLevel.PROTECTED, new GirafImage()) { owner_id = 3 },
				new Pictogram("Harald Blåtand", AccessLevel.PUBLIC, new GirafImage()) { owner_id = 3 }
			};
			foreach (var pictogram in Pictograms)
			{
				context.Add(pictogram);
			}
			context.SaveChanges();

			var users = new GirafUser[]
			{
				new GirafUser("Kurt") { Department_Key = 1},
				new GirafUser("Lee") { Department_Key = 1},
				new GirafUser("Harald Gråtand") {Department_Key = 2}
			};
			foreach(var user in users)
			{
				context.Users.Add(user);
			}
			context.SaveChanges();

			var Departments = new Department[]
			{
				new Department { Name = "Tobias' stue for godt humør"}
			};

		}
    }
}
