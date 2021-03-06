﻿// <auto-generated />
using System;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GirafRest.Migrations
{
    [DbContext(typeof(GirafDbContext))]
    [Migration("20200422090502_PictogramtextMigration")]
    partial class PictogramtextMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GirafRest.GuardianRelation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CitizenId")
                        .IsRequired();

                    b.Property<string>("GuardianId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CitizenId");

                    b.HasIndex("GuardianId");

                    b.ToTable("GuardianRelations");
                });

            modelBuilder.Entity("GirafRest.Models.Activity", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Order");

                    b.Property<long>("OtherKey");

                    b.Property<long>("PictogramKey");

                    b.Property<int>("State");

                    b.Property<long?>("TimerKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("PictogramKey");

                    b.HasIndex("TimerKey");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("GirafRest.Models.Department", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Key");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("GirafRest.Models.DepartmentResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("OtherKey");

                    b.Property<long>("PictogramKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("PictogramKey");

                    b.ToTable("DepartmentResources");
                });

            modelBuilder.Entity("GirafRest.Models.GirafRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("GirafRest.Models.GirafUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<long?>("DepartmentKey");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsDepartment");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<long?>("SettingsKey");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<byte[]>("UserIcon");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("DepartmentKey");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("SettingsKey");

                    b.HasIndex("Id", "UserName")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GirafRest.Models.Pictogram", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("AccessLevel");

                    b.Property<string>("ImageHash")
                        .HasColumnName("ImageHash");

                    b.Property<DateTime>("LastEdit");

                    b.Property<byte[]>("Sound");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Id", "Title")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Pictograms");
                });

            modelBuilder.Entity("GirafRest.Models.Setting", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ActivitiesCount");

                    b.Property<int>("CancelMark");

                    b.Property<int>("CompleteMark");

                    b.Property<int>("DefaultTimer");

                    b.Property<bool>("GreyScale");

                    b.Property<int?>("NrOfDaysToDisplay");

                    b.Property<int>("Orientation");

                    b.Property<bool>("PictogramText");

                    b.Property<int>("Theme");

                    b.Property<int?>("TimerSeconds");

                    b.HasKey("Key");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("GirafRest.Models.Timer", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("FullLength");

                    b.Property<bool>("Paused");

                    b.Property<long>("Progress");

                    b.Property<long>("StartTime");

                    b.HasKey("Key");

                    b.ToTable("Timers");
                });

            modelBuilder.Entity("GirafRest.Models.UserResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OtherKey")
                        .IsRequired();

                    b.Property<long>("PictogramKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("PictogramKey");

                    b.ToTable("UserResources");
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("GirafUserId");

                    b.Property<string>("Name");

                    b.Property<long?>("ThumbnailId");

                    b.Property<long>("ThumbnailKey");

                    b.Property<int>("WeekNumber");

                    b.Property<int>("WeekYear");

                    b.HasKey("Id");

                    b.HasIndex("GirafUserId");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ThumbnailId");

                    b.ToTable("Weeks");
                });

            modelBuilder.Entity("GirafRest.Models.WeekTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("DepartmentKey");

                    b.Property<string>("Name");

                    b.Property<long?>("ThumbnailId");

                    b.Property<long>("ThumbnailKey");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentKey");

                    b.HasIndex("ThumbnailId");

                    b.ToTable("WeekTemplates");
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("Day");

                    b.Property<long?>("WeekId");

                    b.Property<long?>("WeekTemplateId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("WeekId");

                    b.HasIndex("WeekTemplateId");

                    b.ToTable("Weekdays");
                });

            modelBuilder.Entity("GirafRest.WeekDayColor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Day");

                    b.Property<string>("HexColor");

                    b.Property<long>("SettingId");

                    b.HasKey("Id");

                    b.HasIndex("SettingId");

                    b.ToTable("WeekDayColors");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GirafRest.GuardianRelation", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", "Citizen")
                        .WithMany("Guardians")
                        .HasForeignKey("CitizenId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.GirafUser", "Guardian")
                        .WithMany("Citizens")
                        .HasForeignKey("GuardianId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.Activity", b =>
                {
                    b.HasOne("GirafRest.Models.Weekday", "Other")
                        .WithMany("Activities")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany()
                        .HasForeignKey("PictogramKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Timer", "Timer")
                        .WithMany()
                        .HasForeignKey("TimerKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.DepartmentResource", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Other")
                        .WithMany("Resources")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany("Departments")
                        .HasForeignKey("PictogramKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.GirafUser", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Department")
                        .WithMany("Members")
                        .HasForeignKey("DepartmentKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Setting", "Settings")
                        .WithMany()
                        .HasForeignKey("SettingsKey");
                });

            modelBuilder.Entity("GirafRest.Models.UserResource", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", "Other")
                        .WithMany("Resources")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany("Users")
                        .HasForeignKey("PictogramKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany("WeekSchedule")
                        .HasForeignKey("GirafUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailId");
                });

            modelBuilder.Entity("GirafRest.Models.WeekTemplate", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Department")
                        .WithMany("WeekTemplates")
                        .HasForeignKey("DepartmentKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailId");
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.HasOne("GirafRest.Models.Week")
                        .WithMany("Weekdays")
                        .HasForeignKey("WeekId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.WeekTemplate")
                        .WithMany("Weekdays")
                        .HasForeignKey("WeekTemplateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.WeekDayColor", b =>
                {
                    b.HasOne("GirafRest.Models.Setting", "Setting")
                        .WithMany("WeekDayColors")
                        .HasForeignKey("SettingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
