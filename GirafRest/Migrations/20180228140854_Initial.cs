using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Frames",
                columns: table => new
                {
                    AccessLevel = table.Column<int>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true),
                    Sound = table.Column<byte[]>(nullable: true),
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discriminator = table.Column<string>(nullable: false),
                    LastEdit = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frames", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LauncherOptions",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayLauncherAnimations = table.Column<bool>(nullable: false),
                    UseGrayscale = table.Column<bool>(nullable: false),
                    appGridSizeColumns = table.Column<int>(nullable: false),
                    appGridSizeRows = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LauncherOptions", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChoiceResource",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OtherKey = table.Column<long>(nullable: false),
                    ResourceKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoiceResource", x => x.Key);
                    table.ForeignKey(
                        name: "FK_ChoiceResource_Frames_OtherKey",
                        column: x => x.OtherKey,
                        principalTable: "Frames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChoiceResource_Frames_ResourceKey",
                        column: x => x.ResourceKey,
                        principalTable: "Frames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeparmentResources",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OtherKey = table.Column<long>(nullable: false),
                    ResourceKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeparmentResources", x => x.Key);
                    table.ForeignKey(
                        name: "FK_DeparmentResources_Departments_OtherKey",
                        column: x => x.OtherKey,
                        principalTable: "Departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeparmentResources_Frames_ResourceKey",
                        column: x => x.ResourceKey,
                        principalTable: "Frames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationOption",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationName = table.Column<string>(nullable: false),
                    ApplicationPackage = table.Column<string>(nullable: false),
                    LauncherOptionsKey = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationOption_LauncherOptions_LauncherOptionsKey",
                        column: x => x.LauncherOptionsKey,
                        principalTable: "LauncherOptions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    DepartmentKey = table.Column<long>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    GirafUserId = table.Column<string>(nullable: true),
                    IsDepartment = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    SettingsKey = table.Column<long>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserIcon = table.Column<byte[]>(nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Departments_DepartmentKey",
                        column: x => x.DepartmentKey,
                        principalTable: "Departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_GirafUserId",
                        column: x => x.GirafUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_LauncherOptions_SettingsKey",
                        column: x => x.SettingsKey,
                        principalTable: "LauncherOptions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserResources",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OtherKey = table.Column<string>(nullable: false),
                    ResourceKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResources", x => x.Key);
                    table.ForeignKey(
                        name: "FK_UserResources_AspNetUsers_OtherKey",
                        column: x => x.OtherKey,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserResources_Frames_ResourceKey",
                        column: x => x.ResourceKey,
                        principalTable: "Frames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discriminator = table.Column<string>(nullable: false),
                    GirafUserId = table.Column<string>(nullable: true),
                    ThumbnailKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Weeks_AspNetUsers_GirafUserId",
                        column: x => x.GirafUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Weeks_Frames_ThumbnailKey",
                        column: x => x.ThumbnailKey,
                        principalTable: "Frames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weekdays",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Day = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ElementsSet = table.Column<bool>(nullable: false),
                    LastEdit = table.Column<DateTime>(nullable: false),
                    WeekId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weekdays", x => x.id);
                    table.ForeignKey(
                        name: "FK_Weekdays_Weeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "Weeks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeekdayResources",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OtherKey = table.Column<long>(nullable: false),
                    ResourceKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekdayResources", x => x.Key);
                    table.ForeignKey(
                        name: "FK_WeekdayResources_Weekdays_OtherKey",
                        column: x => x.OtherKey,
                        principalTable: "Weekdays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeekdayResources_Frames_ResourceKey",
                        column: x => x.ResourceKey,
                        principalTable: "Frames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationOption_LauncherOptionsKey",
                table: "ApplicationOption",
                column: "LauncherOptionsKey");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentKey",
                table: "AspNetUsers",
                column: "DepartmentKey");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_GirafUserId",
                table: "AspNetUsers",
                column: "GirafUserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SettingsKey",
                table: "AspNetUsers",
                column: "SettingsKey");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceResource_OtherKey",
                table: "ChoiceResource",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceResource_ResourceKey",
                table: "ChoiceResource",
                column: "ResourceKey");

            migrationBuilder.CreateIndex(
                name: "IX_DeparmentResources_OtherKey",
                table: "DeparmentResources",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_DeparmentResources_ResourceKey",
                table: "DeparmentResources",
                column: "ResourceKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserResources_OtherKey",
                table: "UserResources",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserResources_ResourceKey",
                table: "UserResources",
                column: "ResourceKey");

            migrationBuilder.CreateIndex(
                name: "IX_WeekdayResources_OtherKey",
                table: "WeekdayResources",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_WeekdayResources_ResourceKey",
                table: "WeekdayResources",
                column: "ResourceKey");

            migrationBuilder.CreateIndex(
                name: "IX_Weekdays_WeekId",
                table: "Weekdays",
                column: "WeekId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_GirafUserId",
                table: "Weeks",
                column: "GirafUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThumbnailKey",
                table: "Weeks",
                column: "ThumbnailKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationOption");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChoiceResource");

            migrationBuilder.DropTable(
                name: "DeparmentResources");

            migrationBuilder.DropTable(
                name: "UserResources");

            migrationBuilder.DropTable(
                name: "WeekdayResources");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Weekdays");

            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Frames");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "LauncherOptions");
        }
    }
}
