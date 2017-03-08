using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GirafWebApi.Contexts;

namespace GirafWebApi.Migrations
{
    [DbContext(typeof(GirafDbContext))]
    partial class GirafDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("GirafWebApi.Models.Department", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Key");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("GirafWebApi.Models.Frame", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Key");

                    b.ToTable("Frame");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Frame");
                });

            modelBuilder.Entity("GirafWebApi.Models.GirafImage", b =>
                {
                    b.Property<long>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.HasKey("Key");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("GirafWebApi.Models.GirafUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<long>("Department_Key");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<long?>("IconKey");

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

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("Department_Key");

                    b.HasIndex("IconKey");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
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

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
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

            modelBuilder.Entity("GirafWebApi.Models.Choice", b =>
                {
                    b.HasBaseType("GirafWebApi.Models.Frame");


                    b.ToTable("Choice");

                    b.HasDiscriminator().HasValue("Choice");
                });

            modelBuilder.Entity("GirafWebApi.Models.PictoFrame", b =>
                {
                    b.HasBaseType("GirafWebApi.Models.Frame");

                    b.Property<long>("Department_Key");

                    b.Property<long>("owner_id");

                    b.HasIndex("Department_Key");

                    b.ToTable("PictoFrame");

                    b.HasDiscriminator().HasValue("PictoFrame");
                });

            modelBuilder.Entity("GirafWebApi.Models.Pictogram", b =>
                {
                    b.HasBaseType("GirafWebApi.Models.PictoFrame");

                    b.Property<long?>("DepartmentKey");

                    b.Property<long?>("ImageKey");

                    b.HasIndex("DepartmentKey");

                    b.HasIndex("ImageKey");

                    b.ToTable("Pictogram");

                    b.HasDiscriminator().HasValue("Pictogram");
                });

            modelBuilder.Entity("GirafWebApi.Models.Sequence", b =>
                {
                    b.HasBaseType("GirafWebApi.Models.PictoFrame");

                    b.Property<long?>("ThumbnailKey");

                    b.HasIndex("ThumbnailKey");

                    b.ToTable("Sequence");

                    b.HasDiscriminator().HasValue("Sequence");
                });

            modelBuilder.Entity("GirafWebApi.Models.GirafUser", b =>
                {
                    b.HasOne("GirafWebApi.Models.Department", "Department")
                        .WithMany("members")
                        .HasForeignKey("Department_Key")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafWebApi.Models.GirafImage", "Icon")
                        .WithMany()
                        .HasForeignKey("IconKey");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GirafWebApi.Models.GirafUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GirafWebApi.Models.GirafUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GirafWebApi.Models.GirafUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafWebApi.Models.PictoFrame", b =>
                {
                    b.HasOne("GirafWebApi.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("Department_Key")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GirafWebApi.Models.Pictogram", b =>
                {
                    b.HasOne("GirafWebApi.Models.Department")
                        .WithMany("pictograms")
                        .HasForeignKey("DepartmentKey");

                    b.HasOne("GirafWebApi.Models.GirafImage", "Image")
                        .WithMany()
                        .HasForeignKey("ImageKey");
                });

            modelBuilder.Entity("GirafWebApi.Models.Sequence", b =>
                {
                    b.HasOne("GirafWebApi.Models.Pictogram", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailKey");
                });
        }
    }
}
