using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Data
{
    /// <summary>
    /// The GirafDbContext, this is the Database Context for the Giraf database, it defines the various relations between objects in the database.
    /// and which objects exist.
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
        public virtual DbSet<GuardianRelation> GuardianRelations { get; set; }
        public new virtual DbSet<GirafUser> Users { get { return base.Users; } set { base.Users = value; } }
        public new virtual DbSet<GirafRole> Roles { get { return base.Roles; } set { base.Roles = value; } }
        public new virtual DbSet<IdentityUserRole<string>> UserRoles { get { return base.UserRoles; } set { base.UserRoles = value; } }

        protected GirafDbContext() { }
        /// <summary>
        /// Constructor for use when deployed and using the MySql database
        /// </summary>
        /// <param name="options">DbContext options when the MySql database is in use</param>
        public GirafDbContext(DbContextOptions<GirafDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures entity to the needs of this project.
        /// </summary>
        /// <param name="builder">A database model builder, that defines methods for specifying the database design.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Indexes
            builder.Entity<Department>().HasIndex(dep => dep.Name).IsUnique().ForSqlServerIsClustered();
            builder.Entity<Pictogram>().HasIndex(pic => new {pic.Id, pic.Title}).IsUnique().ForSqlServerIsClustered();
            builder.Entity<Weekday>().HasIndex(day => new {day.Id}).IsUnique().ForSqlServerIsClustered();
            builder.Entity<Week>().HasIndex(week => week.Id).IsUnique().ForSqlServerIsClustered();
            builder.Entity<GirafUser>().HasIndex(user => new { user.Id, user.UserName }).IsUnique().ForSqlServerIsClustered();

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
                .HasForeignKey(u => u.OtherKey);
            builder.Entity<UserResource>()
                .HasOne(ur => ur.Pictogram)
                .WithMany(r => r.Users)
                .HasForeignKey(dr => dr.PictogramKey);

            //Configure a One-to-Many relationship between user and department
            builder.Entity<GirafUser>()
                .HasOne<Department>(u => u.Department)
                .WithMany(d => d.Members);

            builder.Entity<Department>()
                .HasMany<WeekTemplate>(u => u.WeekTemplates)
                .WithOne(d => d.Department);

            //And a One-to-Many relationship between Week and Weekday
            builder.Entity<Weekday>()
                .HasOne<Week>()
                .WithMany(w => w.Weekdays);

            //And a One-to-Many relationship between WeekTemplate and Weekday
            builder.Entity<Weekday>()
                .HasOne<WeekTemplate>()
                .WithMany(w => w.Weekdays);

            //Configure a many-to-many relationship between Weekday and Resource(Pictogram)
            builder.Entity<Activity>()
                .HasOne<Weekday>(wr => wr.Other)
                   .WithMany(w => w.Activities)
                .HasForeignKey(wr => wr.OtherKey);
            builder.Entity<Activity>()
                .HasOne<Pictogram>(wr => wr.Pictogram)
                .WithMany()
                .HasForeignKey(wr => wr.PictogramKey);
            
            // Configure that a citizen can have many guardians and that a citizen can have many guardians
            builder.Entity<GuardianRelation>()
                   .HasOne(gr => gr.Guardian)
                   .WithMany(g => g.Citizens)
                   .HasForeignKey(go => go.GuardianId);

            builder.Entity<GuardianRelation>()
                   .HasOne(gr => gr.Citizen)
                   .WithMany(c => c.Guardians)
                   .HasForeignKey(mg => mg.CitizenId);
        }
    }
}
