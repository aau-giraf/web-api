using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using GirafRest.Models.Many_to_Many_Relationships;

namespace GirafRest.Data
{
    public class GirafDbContext : IdentityDbContext<GirafUser>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Pictogram> Pictograms { get; set; }
        public DbSet<PictoFrame> PictoFrames { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Week> Weeks { get; set; }
        public DbSet<Frame> Frames {get; set;}
        public DbSet<UserResource> UserResources { get; set; }
        public DbSet<DepartmentResource> DepartmentResources { get; set; }
        public DbSet<WeekdayResource> WeekdayResources {get; set;}

        public GirafDbContext(DbContextOptions<GirafDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Frame>().ToTable("Frames").HasDiscriminator<string>("Discriminator").HasValue<Frame>(nameof(Frame));
            builder.Entity<Department>().ToTable("Departments").HasDiscriminator<string>("Discriminator").HasValue<Department>(nameof(Department));
            builder.Entity<Pictogram>().ToTable("Pictograms").HasDiscriminator<string>("Discriminator").HasValue<Pictogram>(nameof(Pictogram));
            builder.Entity<PictoFrame>().ToTable("PictoFrames").HasDiscriminator<string>("Discriminator").HasValue<PictoFrame>(nameof(PictoFrame));
            builder.Entity<Choice>().ToTable("Choices").HasDiscriminator<string>("Discriminator").HasValue<Choice>(nameof(Choice));
            builder.Entity<Weekday>().ToTable("Weekdays").HasDiscriminator<string>("Discriminator").HasValue<Weekday>(nameof(Weekday));

            //asp.net does not support many-to-many in its' current release. Here is a work around.
            //The work around is similar to the one taught in the DBS course, where a relationship called
            //DeparmentResource is used to map between departments and resources
            builder.Entity<DepartmentResource>()
                .HasOne(dr => dr.Other)
                .WithMany(d => d.Resources)
                .HasForeignKey(dr => dr.OtherKey);
            builder.Entity<DepartmentResource>()
                .HasOne(dr => dr.Resource)
                .WithMany(r => r.Departments)
                .HasForeignKey(dr => dr.ResourceKey);
            //The same goes for user and resources
            builder.Entity<UserResource>()
                .HasOne(ur => ur.Other)
                .WithMany(u => u.Resources)
                .HasForeignKey(u => u.OtherKey);
            builder.Entity<UserResource>()
                .HasOne(ur => ur.Resource)
                .WithMany(r => r.Users)
                .HasForeignKey(dr => dr.ResourceKey);

            builder.Entity<GirafUser>()
                .HasOne<Department>(u => u.Department)
                .WithMany(d => d.Members);

            builder.Entity<Weekday>()
                .HasOne<Week>()
                .WithMany(w => w.Weekdays);

            builder.Entity<WeekdayResource>()
                .HasOne<Weekday>(wr => wr.Other)
                .WithMany(w => w.Elements)
                .HasForeignKey(wr => wr.OtherKey);
                
            builder.Entity<WeekdayResource>()
                .HasOne<Frame> (wr => wr.Resource)
                .WithMany()
                .HasForeignKey(wr => wr.ResourceKey);

            builder.Entity<ChoiceResource>()
                .HasOne<Choice>(cr => cr.Other)
                .WithMany(c => c.Options)
                .HasForeignKey(cr => cr.OtherKey);

            builder.Entity<ChoiceResource>()
                .HasOne<Frame>(cr => cr.Resource)
                .WithMany()
                .HasForeignKey(cr => cr.ResourceKey);
            
            builder.Entity<UserResource>().ToTable("UserResources");
            builder.Entity<DepartmentResource>().ToTable("DeparmentResources");
            builder.Entity<WeekdayResource>().ToTable("WeekdayResources");
            //builder.Entity<Weekday>().ToTable("Weekdays").HasDiscriminator<string>("Discriminator").HasValue<Weekday>(nameof(Weekday));
            builder.Entity<Week>().ToTable("Weeks").HasDiscriminator<string>("Discriminator").HasValue<Week>(nameof(Week));
            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
