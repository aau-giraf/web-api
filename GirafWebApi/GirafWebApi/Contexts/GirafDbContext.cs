using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafWebApi.Models;
using Microsoft.Data.Sqlite;

namespace GirafWebApi.Contexts
{
    public class GirafDbContext : IdentityDbContext<GirafUser>
    {
        public GirafDbContext(DbContextOptions<GirafDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "Caskit.db" };
        	var connectionString = connectionStringBuilder.ToString();
        	var connection = new SqliteConnection(connectionString);

			optionsBuilder.UseSqlite(connection);
		}
    }
}
