using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafWebApi.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using System.Xml.Linq;

namespace GirafWebApi.Contexts
{
    public class BloggingContextFactory : IDbContextFactory<GirafDbContext>
    {
        public GirafDbContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GirafDbContext>();
            if(Program.DbOption == DbOption.SQLite) {
                var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "GirafDB.db" };
                var connectionString = connectionStringBuilder.ToString();
                var connection = new SqliteConnection(connectionString);

                System.Console.WriteLine("Configured the local database.");
                optionsBuilder.UseSqlite(connection);
            }
            else if (Program.DbOption == DbOption.SQL) {
                //Open the XML document on the specified path and check that the file actually exists
                XDocument config = XDocument.Load(new System.Uri(Program.ConfigurationFilePath).AbsoluteUri);
                if(config == null) {
                    throw new FileNotFoundException("Failed to find a suitable XML-based configuration file");
                }
                //Extract the connection string
                var connString = config.Element(Program.CONNECTIONSTRING_NAME);
                if(connString == null) {
                    throw new ArgumentNullException($"The XML file on specified path must contain an element called {Program.CONNECTIONSTRING_NAME}.\nThe given path was: {Program.ConfigurationFilePath}");
                }

                //Setup the connection to the sql server
                optionsBuilder.UseSqlServer(connString.ToString());
            }

            return new GirafDbContext(optionsBuilder.Options);
        }

        public GirafDbContext Create(DbContextFactoryOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class GirafDbContext : IdentityDbContext<GirafUser>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Pictogram> Pictograms { get; set; }
        public DbSet<PictoFrame> PictoFrames { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Frame> Frames { get; set; }
        public DbSet<Sequence> Sequences { get; set; }

        public GirafDbContext(DbContextOptions<GirafDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Department>().ToTable("Departments");
            builder.Entity<Pictogram>().ToTable("Pictograms");
            builder.Entity<PictoFrame>().ToTable("PictoFrames");
            builder.Entity<Frame>().ToTable("Frames");
            builder.Entity<Choice>().ToTable("Choices");
            builder.Entity<Sequence>().ToTable("Sequences");
            builder.Entity<IdentityRole>().ToTable("Roles");
            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
