﻿// <auto-generated />
using System;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GirafRest.Migrations
{
    [DbContext(typeof(GirafDbContext))]
    [Migration("20221006105822_AddedPopupFlagToDB")]
    partial class AddedPopupFlagToDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GirafRest.Models.Activity", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("ChoiceBoardName")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsChoiceBoard")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<long>("OtherKey")
                        .HasColumnType("bigint");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<long?>("TimerKey")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("TimerKey");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("GirafRest.Models.AlternateName", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("CitizenId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext")
                        .HasColumnName("Name");

                    b.Property<long>("PictogramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CitizenId");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("PictogramId");

                    b.ToTable("AlternateNames");
                });

            modelBuilder.Entity("GirafRest.Models.Department", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Key");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("GirafRest.Models.DepartmentResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("OtherKey")
                        .HasColumnType("bigint");

                    b.Property<long>("PictogramKey")
                        .HasColumnType("bigint");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("PictogramKey");

                    b.ToTable("DepartmentResources");
                });

            modelBuilder.Entity("GirafRest.Models.GirafRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("GirafRest.Models.GirafUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<long?>("DepartmentKey")
                        .HasColumnType("bigint");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDepartment")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<long?>("SettingsKey")
                        .HasColumnType("bigint");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<byte[]>("UserIcon")
                        .HasColumnType("longblob");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentKey");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("SettingsKey");

                    b.HasIndex("Id", "UserName")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GirafRest.Models.GuardianRelation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("CitizenId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("GuardianId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CitizenId");

                    b.HasIndex("GuardianId");

                    b.ToTable("GuardianRelations");
                });

            modelBuilder.Entity("GirafRest.Models.Pictogram", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<int>("AccessLevel")
                        .HasColumnType("int");

                    b.Property<string>("ImageHash")
                        .HasColumnType("longtext")
                        .HasColumnName("ImageHash");

                    b.Property<DateTime>("LastEdit")
                        .HasColumnType("datetime");

                    b.Property<byte[]>("Sound")
                        .HasColumnType("longblob");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Id", "Title")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Pictograms");
                });

            modelBuilder.Entity("GirafRest.Models.PictogramRelation", b =>
                {
                    b.Property<long>("ActivityId")
                        .HasColumnType("bigint");

                    b.Property<long>("PictogramId")
                        .HasColumnType("bigint");

                    b.HasKey("ActivityId", "PictogramId");

                    b.HasIndex("PictogramId");

                    b.ToTable("PictogramRelations");
                });

            modelBuilder.Entity("GirafRest.Models.Setting", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int?>("ActivitiesCount")
                        .HasColumnType("int");

                    b.Property<int>("CancelMark")
                        .HasColumnType("int");

                    b.Property<int>("CompleteMark")
                        .HasColumnType("int");

                    b.Property<int>("DefaultTimer")
                        .HasColumnType("int");

                    b.Property<bool>("GreyScale")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockTimerControl")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("NrOfDaysToDisplay")
                        .HasColumnType("int");

                    b.Property<int>("Orientation")
                        .HasColumnType("int");

                    b.Property<bool>("PictogramText")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("ShowPopup")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Theme")
                        .HasColumnType("int");

                    b.Property<int?>("TimerSeconds")
                        .HasColumnType("int");

                    b.HasKey("Key");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("GirafRest.Models.Timer", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("FullLength")
                        .HasColumnType("bigint");

                    b.Property<bool>("Paused")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("Progress")
                        .HasColumnType("bigint");

                    b.Property<long>("StartTime")
                        .HasColumnType("bigint");

                    b.HasKey("Key");

                    b.ToTable("Timers");
                });

            modelBuilder.Entity("GirafRest.Models.UserResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("OtherKey")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<long>("PictogramKey")
                        .HasColumnType("bigint");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("PictogramKey");

                    b.ToTable("UserResources");
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("GirafUserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<long>("ThumbnailKey")
                        .HasColumnType("bigint");

                    b.Property<int>("WeekNumber")
                        .HasColumnType("int");

                    b.Property<int>("WeekYear")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GirafUserId");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ThumbnailKey");

                    b.ToTable("Weeks");
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<long?>("WeekId")
                        .HasColumnType("bigint");

                    b.Property<long?>("WeekTemplateId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("WeekId");

                    b.HasIndex("WeekTemplateId");

                    b.ToTable("Weekdays");
                });

            modelBuilder.Entity("GirafRest.Models.WeekDayColor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<string>("HexColor")
                        .HasColumnType("longtext");

                    b.Property<long>("SettingId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SettingId");

                    b.ToTable("WeekDayColors");
                });

            modelBuilder.Entity("GirafRest.Models.WeekTemplate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("DepartmentKey")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<long>("ThumbnailKey")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentKey");

                    b.HasIndex("ThumbnailKey");

                    b.ToTable("WeekTemplates");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("GirafRest.Models.Activity", b =>
                {
                    b.HasOne("GirafRest.Models.Weekday", "Other")
                        .WithMany("Activities")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.Timer", "Timer")
                        .WithMany()
                        .HasForeignKey("TimerKey")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Other");

                    b.Navigation("Timer");
                });

            modelBuilder.Entity("GirafRest.Models.AlternateName", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", "Citizen")
                        .WithMany()
                        .HasForeignKey("CitizenId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany("AlternateNames")
                        .HasForeignKey("PictogramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Citizen");

                    b.Navigation("Pictogram");
                });

            modelBuilder.Entity("GirafRest.Models.DepartmentResource", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Other")
                        .WithMany("Resources")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany("Departments")
                        .HasForeignKey("PictogramKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Other");

                    b.Navigation("Pictogram");
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

                    b.Navigation("Department");

                    b.Navigation("Settings");
                });

            modelBuilder.Entity("GirafRest.Models.GuardianRelation", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", "Citizen")
                        .WithMany("Guardians")
                        .HasForeignKey("CitizenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.GirafUser", "Guardian")
                        .WithMany("Citizens")
                        .HasForeignKey("GuardianId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Citizen");

                    b.Navigation("Guardian");
                });

            modelBuilder.Entity("GirafRest.Models.PictogramRelation", b =>
                {
                    b.HasOne("GirafRest.Models.Activity", "Activity")
                        .WithMany("Pictograms")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany("Activities")
                        .HasForeignKey("PictogramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("Pictogram");
                });

            modelBuilder.Entity("GirafRest.Models.UserResource", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", "Other")
                        .WithMany("Resources")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.Pictogram", "Pictogram")
                        .WithMany("Users")
                        .HasForeignKey("PictogramKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Other");

                    b.Navigation("Pictogram");
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", null)
                        .WithMany("WeekSchedule")
                        .HasForeignKey("GirafUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Pictogram", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Thumbnail");
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.HasOne("GirafRest.Models.Week", null)
                        .WithMany("Weekdays")
                        .HasForeignKey("WeekId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.WeekTemplate", null)
                        .WithMany("Weekdays")
                        .HasForeignKey("WeekTemplateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.WeekDayColor", b =>
                {
                    b.HasOne("GirafRest.Models.Setting", "Setting")
                        .WithMany("WeekDayColors")
                        .HasForeignKey("SettingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Setting");
                });

            modelBuilder.Entity("GirafRest.Models.WeekTemplate", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Department")
                        .WithMany("WeekTemplates")
                        .HasForeignKey("DepartmentKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.Pictogram", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Thumbnail");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GirafRest.Models.GirafUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GirafRest.Models.Activity", b =>
                {
                    b.Navigation("Pictograms");
                });

            modelBuilder.Entity("GirafRest.Models.Department", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Resources");

                    b.Navigation("WeekTemplates");
                });

            modelBuilder.Entity("GirafRest.Models.GirafUser", b =>
                {
                    b.Navigation("Citizens");

                    b.Navigation("Guardians");

                    b.Navigation("Resources");

                    b.Navigation("WeekSchedule");
                });

            modelBuilder.Entity("GirafRest.Models.Pictogram", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("AlternateNames");

                    b.Navigation("Departments");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("GirafRest.Models.Setting", b =>
                {
                    b.Navigation("WeekDayColors");
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.Navigation("Weekdays");
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.Navigation("Activities");
                });

            modelBuilder.Entity("GirafRest.Models.WeekTemplate", b =>
                {
                    b.Navigation("Weekdays");
                });
#pragma warning restore 612, 618
        }
    }
}
