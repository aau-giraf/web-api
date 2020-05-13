using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable 1591
namespace GirafRest.Data
{
    /// <summary>
    /// The GirafDbContext, this is the Database Context for the Giraf database, it defines the various relations between objects in the database.
    /// and which objects exist.
    /// By convention each DbSet will create a table in the database with the given name
    /// </summary>
    public class GirafDbContext : IdentityDbContext<GirafUser, GirafRole, string>
    {
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Pictogram> Pictograms { get; set; }
        public virtual DbSet<Week> Weeks { get; set; }
        public virtual DbSet<WeekTemplate> WeekTemplates { get; set; }
        public virtual DbSet<Weekday> Weekdays { get; set; }
        public virtual DbSet<UserResource> UserResources { get; set; }
        public virtual DbSet<DepartmentResource> DepartmentResources { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Timer> Timers { get; set; }
        public virtual DbSet<GuardianRelation> GuardianRelations { get; set; }
        public virtual DbSet<WeekDayColor> WeekDayColors { get; set; }
        protected GirafDbContext() { }

        public GirafDbContext(DbContextOptions<GirafDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures entity to the needs of this project through the fluent API
        /// </summary>
        /// <param name="builder">A database model builder, that defines methods for specifying the database design.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            if (builder == null) {
                throw new System.ArgumentNullException(builder + " is null");
            }
            //Indexes
            builder.Entity<Department>().HasIndex(dep => dep.Name).IsUnique().IsClustered();
            builder.Entity<Pictogram>().HasIndex(pic => new {pic.Id, pic.Title}).IsUnique().IsClustered();
            builder.Entity<Weekday>().HasIndex(day => new {day.Id}).IsUnique().IsClustered();
            builder.Entity<Week>().HasIndex(week => week.Id).IsUnique().IsClustered();
            builder.Entity<GirafUser>().HasIndex(user => new { user.Id, user.UserName }).IsUnique().IsClustered();

            builder.Entity<DepartmentResource>()
                //States that one department is involved in each DepartmentResourceRelation
                .HasOne(dr => dr.Other)
                //And that this one department may own several DepartmentResources
                .WithMany(d => d.Resources)
                //And that the key of the department in the relation is 'OtherKey'
                .HasForeignKey(dr => dr.OtherKey);
            //Configures the relation from Resource to DepartmentResource

            builder.Entity<DepartmentResource>()
                //States that only one resource is involved in this relation
                .HasOne(dr => dr.Pictogram)
                //And that each resource may take part in many DepartmentResource relations
                .WithMany(r => r.Departments)
                .HasForeignKey(dr => dr.PictogramKey);
            //The same goes for user and resources

            builder.Entity<UserResource>()
                .HasOne(ur => ur.Other)
                .WithMany(u => u.Resources)
                .HasForeignKey(u => u.OtherKey)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<UserResource>()
                .HasOne(ur => ur.Pictogram)
                .WithMany(r => r.Users)
                .HasForeignKey(dr => dr.PictogramKey)
                .OnDelete(DeleteBehavior.Cascade);

            //Configure a One-to-Many relationship between user and department
            builder.Entity<GirafUser>()
                .HasOne<Department>(u => u.Department)
                .WithMany(d => d.Members)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Department>()
                .HasMany<WeekTemplate>(u => u.WeekTemplates)
                .WithOne(d => d.Department)
                .OnDelete(DeleteBehavior.Cascade);

            //And a One-to-Many relationship between Week and Weekday
            builder.Entity<Weekday>()
                .HasOne<Week>()
                .WithMany(w => w.Weekdays)
                .OnDelete(DeleteBehavior.Cascade);

            //And a One-to-Many relationship between WeekTemplate and Weekday
            builder.Entity<Weekday>()
                .HasOne<WeekTemplate>()
                .WithMany(w => w.Weekdays)
                .OnDelete(DeleteBehavior.Cascade);

            //Configure a many-to-many relationship between Weekday and Resource(Pictogram)
            builder.Entity<Activity>()
                .HasOne<Weekday>(wr => wr.Other)
                   .WithMany(w => w.Activities)
                .HasForeignKey(wr => wr.OtherKey)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Activity>()
                .HasOne<Pictogram>(wr => wr.Pictogram)
                .WithMany()
                .HasForeignKey(wr => wr.PictogramKey)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Activity>()
                .HasOne<Timer>(ac => ac.Timer)
                .WithMany()
                .HasForeignKey(ac => ac.TimerKey)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure that a citizen can have many guardians and that a citizen can have many guardians
            builder.Entity<GuardianRelation>()
                   .HasOne(gr => gr.Guardian)
                   .WithMany(g => g.Citizens)
                   .HasForeignKey(go => go.GuardianId)
                   .OnDelete(DeleteBehavior.Cascade);

            // The pivot table for the many-many between citizens and guardians (configure one-to-many on the one side)
            builder.Entity<GuardianRelation>()
                   .HasOne(gr => gr.Citizen)
                   .WithMany(c => c.Guardians)
                   .HasForeignKey(mg => mg.CitizenId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure a one-to-many relationship setting and weekdaycolors
            builder.Entity<Setting>()
                   .HasMany(gr => gr.WeekDayColors)
                   .WithOne(c => c.Setting)
                   .HasForeignKey(s => s.SettingId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure a one-to-many relationship between user and weeks
            builder.Entity<GirafUser>()
                .HasMany<Week>(u => u.WeekSchedule)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
#pragma warning restore 1591