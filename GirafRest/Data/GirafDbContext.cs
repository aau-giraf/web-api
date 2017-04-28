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
    public class GirafDbContext : IdentityDbContext<GirafUser, GirafRole, string>
    {
        public virtual DbSet<Department> Departments { get; set; }
        public virtual  DbSet<Pictogram> Pictograms { get; set; }
        public virtual DbSet<PictoFrame> PictoFrames { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<Week> Weeks { get; set; }
        public virtual DbSet<Resource> Frames {get; set;}
        public virtual DbSet<UserResource> UserResources { get; set; }
        public virtual DbSet<DepartmentResource> DepartmentResources { get; set; }
        public virtual DbSet<WeekdayResource> WeekdayResources {get; set;}

        protected GirafDbContext () {}
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
            //Creates tables for all entities.
            builder.Entity<Resource>().ToTable("Frames").HasDiscriminator<string>("Discriminator").HasValue<Resource>(nameof(Resource));
            builder.Entity<Department>().ToTable("Departments").HasDiscriminator<string>("Discriminator").HasValue<Department>(nameof(Department));
            builder.Entity<Pictogram>().ToTable("Pictograms").HasDiscriminator<string>("Discriminator").HasValue<Pictogram>(nameof(Pictogram));
            builder.Entity<PictoFrame>().ToTable("PictoFrames").HasDiscriminator<string>("Discriminator").HasValue<PictoFrame>(nameof(PictoFrame));
            builder.Entity<Choice>().ToTable("Choices").HasDiscriminator<string>("Discriminator").HasValue<Choice>(nameof(Choice));
            builder.Entity<Weekday>().ToTable("Weekdays").HasDiscriminator<string>("Discriminator").HasValue<Weekday>(nameof(Weekday));
            builder.Entity<UserRole>().ToTable("UserRoles").HasDiscriminator<string>("Discriminator").HasValue<UserRole>(nameof(UserRole));

            //asp.net does not support many-to-many in its' current release. Here is a workaround.
            //The workaround is similar to the one taught in the DBS course, where a relationship called
            //DeparmentResource is used to map between departments and resources.
            //This lines configures the relation from Department to DepartmentResource
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
                .HasOne(dr => dr.Resource)
                //And that each resource may take part in many DepartmentResource relations
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

            //Configure a One-to-Many relationship between user and department
            builder.Entity<GirafUser>()
                .HasOne<Department>(u => u.Department)
                .WithMany(d => d.Members);

            //And a One-to-Many relationship between Week and Weekday
            builder.Entity<Weekday>()
                .HasOne<Week>()
                .WithMany(w => w.Weekdays);

            //Configure a many-to-many relationship between Weekday and Resource(PictoFrame)
            builder.Entity<WeekdayResource>()
                .HasOne<Weekday>(wr => wr.Other)
                .WithMany(w => w.Elements)
                .HasForeignKey(wr => wr.OtherKey);
            builder.Entity<WeekdayResource>()
                .HasOne<Resource> (wr => wr.Resource)
                .WithMany()
                .HasForeignKey(wr => wr.ResourceKey);
            //And between Choice and Resource(PictoFrame)
            builder.Entity<ChoiceResource>()
                .HasOne<Choice>(cr => cr.Other)
                .WithMany(c => c.Options)
                .HasForeignKey(cr => cr.OtherKey);
            builder.Entity<ChoiceResource>()
                .HasOne<Resource>(cr => cr.Resource)
                .WithMany()
                .HasForeignKey(cr => cr.ResourceKey);
            
            //Create tables for all many-to-many relationships.
            builder.Entity<UserResource>().ToTable("UserResources");
            builder.Entity<DepartmentResource>().ToTable("DeparmentResources");
            builder.Entity<WeekdayResource>().ToTable("WeekdayResources");
            //builder.Entity<Weekday>().ToTable("Weekdays").HasDiscriminator<string>("Discriminator").HasValue<Weekday>(nameof(Weekday));
            builder.Entity<Week>().ToTable("Weeks").HasDiscriminator<string>("Discriminator").HasValue<Week>(nameof(Week));
        }
    }
}
