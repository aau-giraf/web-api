using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
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
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Pictograms",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    LastEdit = table.Column<DateTime>(nullable: false),
                    AccessLevel = table.Column<int>(nullable: false),
                    ImageHash = table.Column<string>(nullable: true),
                    Sound = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictograms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Orientation = table.Column<int>(nullable: false),
                    CompleteMark = table.Column<int>(nullable: false),
                    CancelMark = table.Column<int>(nullable: false),
                    DefaultTimer = table.Column<int>(nullable: false),
                    TimerSeconds = table.Column<int>(nullable: true),
                    ActivitiesCount = table.Column<int>(nullable: true),
                    Theme = table.Column<int>(nullable: false),
                    NrOfDaysToDisplayPortait = table.Column<int>(nullable: true),
                    DisplayDaysRelativePortait = table.Column<int>(nullable: false),
                    NrOfDaysToDisplayLandscape = table.Column<int>(nullable: true),
                    DisplayDaysRelativeLandscape = table.Column<int>(nullable: false),
                    GreyScale = table.Column<bool>(nullable: false),
                    PictogramText = table.Column<bool>(nullable: false),
                    LockTimerControl = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Timers",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartTime = table.Column<long>(nullable: false),
                    Progress = table.Column<long>(nullable: false),
                    FullLength = table.Column<long>(nullable: false),
                    Paused = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timers", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                name: "DepartmentResources",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OtherKey = table.Column<long>(nullable: false),
                    PictogramKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentResources", x => x.Key);
                    table.ForeignKey(
                        name: "FK_DepartmentResources_Departments_OtherKey",
                        column: x => x.OtherKey,
                        principalTable: "Departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentResources_Pictograms_PictogramKey",
                        column: x => x.PictogramKey,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeekTemplates",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ThumbnailKey = table.Column<long>(nullable: false),
                    DepartmentKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekTemplates", x => x.id);
                    table.ForeignKey(
                        name: "FK_WeekTemplates_Departments_DepartmentKey",
                        column: x => x.DepartmentKey,
                        principalTable: "Departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeekTemplates_Pictograms_ThumbnailKey",
                        column: x => x.ThumbnailKey,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsDepartment = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    UserIcon = table.Column<byte[]>(nullable: true),
                    DepartmentKey = table.Column<long>(nullable: true),
                    SettingsKey = table.Column<long>(nullable: true)
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
                        name: "FK_AspNetUsers_Setting_SettingsKey",
                        column: x => x.SettingsKey,
                        principalTable: "Setting",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeekDayColors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HexColor = table.Column<string>(nullable: true),
                    Day = table.Column<int>(nullable: false),
                    SettingId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDayColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekDayColors_Setting_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Setting",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlternateNames",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CitizenId = table.Column<string>(nullable: true),
                    PictogramId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternateNames", x => x.id);
                    table.ForeignKey(
                        name: "FK_AlternateNames_AspNetUsers_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlternateNames_Pictograms_PictogramId",
                        column: x => x.PictogramId,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                name: "GuardianRelations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CitizenId = table.Column<string>(nullable: false),
                    GuardianId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardianRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuardianRelations_AspNetUsers_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuardianRelations_AspNetUsers_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserResources",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OtherKey = table.Column<string>(nullable: false),
                    PictogramKey = table.Column<long>(nullable: false)
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
                        name: "FK_UserResources_Pictograms_PictogramKey",
                        column: x => x.PictogramKey,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ThumbnailKey = table.Column<long>(nullable: false),
                    WeekYear = table.Column<int>(nullable: false),
                    WeekNumber = table.Column<int>(nullable: false),
                    GirafUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Weeks_AspNetUsers_GirafUserId",
                        column: x => x.GirafUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weeks_Pictograms_ThumbnailKey",
                        column: x => x.ThumbnailKey,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weekdays",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Day = table.Column<int>(nullable: false),
                    WeekId = table.Column<long>(nullable: true),
                    WeekTemplateId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weekdays", x => x.id);
                    table.ForeignKey(
                        name: "FK_Weekdays_Weeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "Weeks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weekdays_WeekTemplates_WeekTemplateId",
                        column: x => x.WeekTemplateId,
                        principalTable: "WeekTemplates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OtherKey = table.Column<long>(nullable: false),
                    TimerKey = table.Column<long>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    IsChoiceBoard = table.Column<bool>(nullable: false),
                    ChoiceBoardName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Activities_Weekdays_OtherKey",
                        column: x => x.OtherKey,
                        principalTable: "Weekdays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Timers_TimerKey",
                        column: x => x.TimerKey,
                        principalTable: "Timers",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PictogramRelations",
                columns: table => new
                {
                    ActivityId = table.Column<long>(nullable: false),
                    PictogramId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictogramRelations", x => new { x.ActivityId, x.PictogramId });
                    table.ForeignKey(
                        name: "FK_PictogramRelations_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PictogramRelations_Pictograms_PictogramId",
                        column: x => x.PictogramId,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_OtherKey",
                table: "Activities",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TimerKey",
                table: "Activities",
                column: "TimerKey");

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNames_CitizenId",
                table: "AlternateNames",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNames_id",
                table: "AlternateNames",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNames_PictogramId",
                table: "AlternateNames",
                column: "PictogramId");

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
                name: "IX_AspNetUsers_Id_UserName",
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResources_OtherKey",
                table: "DepartmentResources",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResources_PictogramKey",
                table: "DepartmentResources",
                column: "PictogramKey");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuardianRelations_CitizenId",
                table: "GuardianRelations",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_GuardianRelations_GuardianId",
                table: "GuardianRelations",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_PictogramRelations_PictogramId",
                table: "PictogramRelations",
                column: "PictogramId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictograms_id_Title",
                table: "Pictograms",
                columns: new[] { "id", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserResources_OtherKey",
                table: "UserResources",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserResources_PictogramKey",
                table: "UserResources",
                column: "PictogramKey");

            migrationBuilder.CreateIndex(
                name: "IX_WeekDayColors_SettingId",
                table: "WeekDayColors",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Weekdays_id",
                table: "Weekdays",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weekdays_WeekId",
                table: "Weekdays",
                column: "WeekId");

            migrationBuilder.CreateIndex(
                name: "IX_Weekdays_WeekTemplateId",
                table: "Weekdays",
                column: "WeekTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_GirafUserId",
                table: "Weeks",
                column: "GirafUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_id",
                table: "Weeks",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThumbnailKey",
                table: "Weeks",
                column: "ThumbnailKey");

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplates_DepartmentKey",
                table: "WeekTemplates",
                column: "DepartmentKey");

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplates_ThumbnailKey",
                table: "WeekTemplates",
                column: "ThumbnailKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternateNames");

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
                name: "DepartmentResources");

            migrationBuilder.DropTable(
                name: "GuardianRelations");

            migrationBuilder.DropTable(
                name: "PictogramRelations");

            migrationBuilder.DropTable(
                name: "UserResources");

            migrationBuilder.DropTable(
                name: "WeekDayColors");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Weekdays");

            migrationBuilder.DropTable(
                name: "Timers");

            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "WeekTemplates");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Pictograms");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Setting");
        }
    }
}
