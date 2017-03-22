using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;

namespace GirafRest.Data
{
    public class GirafDbContext : IdentityDbContext<GirafUser>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Pictogram> Pictograms { get; set; }
        public DbSet<PictoFrame> PictoFrames { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<Frame> Frames {get; set;}

        public GirafDbContext(DbContextOptions<GirafDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Frame>().ToTable("Frames");
            builder.Entity<Department>().ToTable("Departments");
            builder.Entity<Pictogram>().ToTable("Pictograms");
            builder.Entity<PictoFrame>().ToTable("PictoFrames");
            builder.Entity<Choice>().ToTable("Choices");
            builder.Entity<Sequence>().ToTable("Sequences");
            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
