﻿using System;
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
        public DbSet<Weekday> Weekdays { get; set; }
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
            builder.Entity<Frame>().ToTable("Frames").HasDiscriminator<string>("Discriminator");
            builder.Entity<Department>().ToTable("Departments").HasDiscriminator<string>("Discriminator");
            builder.Entity<Pictogram>().ToTable("Pictograms").HasDiscriminator<string>("Discriminator").HasValue<Pictogram>(nameof(Pictogram));
            builder.Entity<PictoFrame>().ToTable("PictoFrames").HasDiscriminator<string>("Discriminator");
            builder.Entity<Choice>().ToTable("Choices").HasDiscriminator<string>("Discriminator").HasValue<Choice>(nameof(Choice));

            /*builder.Entity<Weekday>().Property("ThumbnailKey");
            builder.Entity<Weekday>().HasOne(w => w.Thumbnail).WithMany().HasForeignKey("ThumbnailKey");*/
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

            builder.Entity<WeekdayResource>()
                .HasOne<Weekday>(wr => wr.Other)
                .WithMany(w => w.Elements)
                .HasForeignKey(wr => wr.OtherKey);
            builder.Entity<WeekdayResource>()
                .HasOne<Frame> (wr => wr.Resource)
                .WithMany()
                .HasForeignKey(wr => wr.ResourceKey);
            
            builder.Entity<UserResource>().ToTable("UserResources");
            builder.Entity<DepartmentResource>().ToTable("DeparmentResources");
            builder.Entity<WeekdayResource>().ToTable("WeekdayResources");
            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
