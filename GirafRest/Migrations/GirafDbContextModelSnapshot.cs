using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GirafRest.Data;
using GirafRest.Models;

namespace GirafRest.Migrations
{
    [DbContext(typeof(GirafDbContext))]
    partial class GirafDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("GirafRest.Models.ApplicationOption", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationName")
                        .IsRequired();

                    b.Property<string>("ApplicationPackage")
                        .IsRequired();

                    b.Property<string>("GirafUserId");

                    b.HasKey("Id");

                    b.HasIndex("GirafUserId");

                    b.ToTable("ApplicationOption");
                });

            modelBuilder.Entity("GirafRest.Models.Department", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Key");

                    b.ToTable("Departments");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Department");
                });

            modelBuilder.Entity("GirafRest.Models.DepartmentResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("OtherKey");

                    b.Property<long>("ResourceKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("ResourceKey");

                    b.ToTable("DeparmentResources");
                });

            modelBuilder.Entity("GirafRest.Models.Frame", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTime>("LastEdit");

                    b.HasKey("Id");

                    b.ToTable("Frames");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Frame");
                });

            modelBuilder.Entity("GirafRest.Models.GirafRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("IPAddress");

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

                    b.Property<long>("DepartmentKey");

                    b.Property<bool>("DisplayLauncherAnimations");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

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

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<bool>("UseGrayscale");

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

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GirafRest.Models.Many_to_Many_Relationships.ChoiceResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("OtherKey");

                    b.Property<long>("ResourceKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("ResourceKey");

                    b.ToTable("ChoiceResource");
                });

            modelBuilder.Entity("GirafRest.Models.UserResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OtherKey")
                        .IsRequired();

                    b.Property<long>("ResourceKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("ResourceKey");

                    b.ToTable("UserResources");
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("GirafUserId");

                    b.Property<long>("ThumbnailKey");

                    b.HasKey("Id");

                    b.HasIndex("GirafUserId");

                    b.HasIndex("ThumbnailKey");

                    b.ToTable("Weeks");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Week");
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("Day");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("ElementsSet");

                    b.Property<DateTime>("LastEdit");

                    b.Property<long?>("WeekId");

                    b.HasKey("Id");

                    b.HasIndex("WeekId");

                    b.ToTable("Weekdays");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Weekday");
                });

            modelBuilder.Entity("GirafRest.Models.WeekdayResource", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("OtherKey");

                    b.Property<long>("ResourceKey");

                    b.HasKey("Key");

                    b.HasIndex("OtherKey");

                    b.HasIndex("ResourceKey");

                    b.ToTable("WeekdayResources");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUserRole<string>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GirafRest.Models.Choice", b =>
                {
                    b.HasBaseType("GirafRest.Models.Frame");


                    b.ToTable("Choices");

                    b.HasDiscriminator().HasValue("Choice");
                });

            modelBuilder.Entity("GirafRest.Models.PictoFrame", b =>
                {
                    b.HasBaseType("GirafRest.Models.Frame");

                    b.Property<int>("AccessLevel");

                    b.Property<string>("Title");

                    b.ToTable("PictoFrames");

                    b.HasDiscriminator().HasValue("PictoFrame");
                });

            modelBuilder.Entity("GirafRest.Models.UserRole", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>");


                    b.ToTable("UserRoles");

                    b.HasDiscriminator().HasValue("UserRole");
                });

            modelBuilder.Entity("GirafRest.Models.Pictogram", b =>
                {
                    b.HasBaseType("GirafRest.Models.PictoFrame");

                    b.Property<byte[]>("Image")
                        .HasColumnName("Image");

                    b.ToTable("Pictograms");

                    b.HasDiscriminator().HasValue("Pictogram");
                });

            modelBuilder.Entity("GirafRest.Models.ApplicationOption", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany("AvailableApplications")
                        .HasForeignKey("GirafUserId");
                });

            modelBuilder.Entity("GirafRest.Models.DepartmentResource", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Other")
                        .WithMany("Resources")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Frame", "Resource")
                        .WithMany("Departments")
                        .HasForeignKey("ResourceKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.GirafUser", b =>
                {
                    b.HasOne("GirafRest.Models.Department", "Department")
                        .WithMany("Members")
                        .HasForeignKey("DepartmentKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.Many_to_Many_Relationships.ChoiceResource", b =>
                {
                    b.HasOne("GirafRest.Models.Choice", "Other")
                        .WithMany("Options")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Frame", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.UserResource", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser", "Other")
                        .WithMany("Resources")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Frame", "Resource")
                        .WithMany("Users")
                        .HasForeignKey("ResourceKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.Week", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany("WeekSchedule")
                        .HasForeignKey("GirafUserId");

                    b.HasOne("GirafRest.Models.Pictogram", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafRest.Models.Weekday", b =>
                {
                    b.HasOne("GirafRest.Models.Week")
                        .WithMany("Weekdays")
                        .HasForeignKey("WeekId");
                });

            modelBuilder.Entity("GirafRest.Models.WeekdayResource", b =>
                {
                    b.HasOne("GirafRest.Models.Weekday", "Other")
                        .WithMany("Elements")
                        .HasForeignKey("OtherKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.Frame", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("GirafRest.Models.GirafRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafRest.Models.GirafUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
