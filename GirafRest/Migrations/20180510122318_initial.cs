using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    /// <summary>
    /// Initial Migration
    /// </summary>
    public partial class initial : Migration
    {
        /// <summary>
        /// Run migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => (
                    Id: table.Column<string>(nullable: false),
                    ConcurrencyStamp: table.Column<string>(nullable: true),
                    Name: table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName: table.Column<string>(maxLength: 256, nullable: true)
                ),
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => (
                    id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name: table.Column<string>(nullable: false)
                ),
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Pictograms",
                columns: table => (
                    id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccessLevel: table.Column<int>(nullable: false),
                    Image: table.Column<byte[]>(nullable: true),
                    LastEdit: table.Column<DateTime>(nullable: false),
                    Sound: table.Column<byte[]>(nullable: true),
                    Title: table.Column<string>(nullable: false)
                ),
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictograms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => (
                    Key: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivitiesCount: table.Column<int>(nullable: true),
                    CancelMark: table.Column<int>(nullable: false),
                    CompleteMark: table.Column<int>(nullable: false),
                    DefaultTimer: table.Column<int>(nullable: false),
                    GreyScale: table.Column<bool>(nullable: false),
                    NrOfDaysToDisplay: table.Column<int>(nullable: true),
                    Orientation: table.Column<int>(nullable: false),
                    Theme: table.Column<int>(nullable: false),
                    TimerSeconds: table.Column<int>(nullable: true)
                ),
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => (
                    Id: table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClaimType: table.Column<string>(nullable: true),
                    ClaimValue: table.Column<string>(nullable: true),
                    RoleId: table.Column<string>(nullable: false)
                ),
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
                columns: table => (
                    Key: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OtherKey: table.Column<long>(nullable: false),
                    PictogramKey: table.Column<long>(nullable: false),
                    ResourceKey: table.Column<long>(nullable: true)
                ),
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
                columns: table => (
                    id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DepartmentKey: table.Column<long>(nullable: false),
                    Name: table.Column<string>(nullable: true),
                    ThumbnailKey: table.Column<long>(nullable: false)
                ),
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
                columns: table => (
                    Id: table.Column<string>(nullable: false),
                    AccessFailedCount: table.Column<int>(nullable: false),
                    ConcurrencyStamp: table.Column<string>(nullable: true),
                    DepartmentKey: table.Column<long>(nullable: true),
                    DisplayName: table.Column<string>(nullable: true),
                    Email: table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed: table.Column<bool>(nullable: false),
                    IsDepartment: table.Column<bool>(nullable: false),
                    LockoutEnabled: table.Column<bool>(nullable: false),
                    LockoutEnd: table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail: table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName: table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash: table.Column<string>(nullable: true),
                    PhoneNumber: table.Column<string>(nullable: true),
                    PhoneNumberConfirmed: table.Column<bool>(nullable: false),
                    SecurityStamp: table.Column<string>(nullable: true),
                    SettingsKey: table.Column<long>(nullable: true),
                    TwoFactorEnabled: table.Column<bool>(nullable: false),
                    UserIcon: table.Column<byte[]>(nullable: true),
                    UserName: table.Column<string>(maxLength: 256, nullable: true)
                ),
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
                columns: table => (
                    Id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Day: table.Column<int>(nullable: false),
                    HexColor: table.Column<string>(nullable: true),
                    SettingId: table.Column<long>(nullable: false)
                ),
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
                name: "AspNetUserClaims",
                columns: table => (
                    Id: table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClaimType: table.Column<string>(nullable: true),
                    ClaimValue: table.Column<string>(nullable: true),
                    UserId: table.Column<string>(nullable: false)
                ),
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
                columns: table => (
                    LoginProvider: table.Column<string>(nullable: false),
                    ProviderKey: table.Column<string>(nullable: false),
                    ProviderDisplayName: table.Column<string>(nullable: true),
                    UserId: table.Column<string>(nullable: false)
                ),
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
                columns: table => (
                    UserId: table.Column<string>(nullable: false),
                    RoleId: table.Column<string>(nullable: false)
                ),
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
                columns: table => (
                    UserId: table.Column<string>(nullable: false),
                    LoginProvider: table.Column<string>(nullable: false),
                    Name: table.Column<string>(nullable: false),
                    Value: table.Column<string>(nullable: true)
                ),
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
                columns: table => (
                    Id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CitizenId: table.Column<string>(nullable: false),
                    GuardianId: table.Column<string>(nullable: false)
                ),
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
                columns: table => (
                    Key: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OtherKey: table.Column<string>(nullable: false),
                    PictogramKey: table.Column<long>(nullable: false),
                    ResourceKey: table.Column<long>(nullable: true)
                ),
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
                columns: table => (
                    id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GirafUserId: table.Column<string>(nullable: true),
                    Name: table.Column<string>(nullable: true),
                    ThumbnailKey: table.Column<long>(nullable: false),
                    WeekNumber: table.Column<int>(nullable: false),
                    WeekYear: table.Column<int>(nullable: false)
                ),
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
                columns: table => (
                    id: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Day: table.Column<int>(nullable: false),
                    WeekId: table.Column<long>(nullable: true),
                    WeekTemplateId: table.Column<long>(nullable: true)
                ),
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
                columns: table => (
                    Key: table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Order: table.Column<int>(nullable: false),
                    OtherKey: table.Column<long>(nullable: false),
                    PictogramKey: table.Column<long>(nullable: false),
                    ResourceKey: table.Column<long>(nullable: true),
                    State: table.Column<int>(nullable: false)
                ),
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
                        name: "FK_Activities_Pictograms_PictogramKey",
                        column: x => x.PictogramKey,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_OtherKey",
                table: "Activities",
                column: "OtherKey");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PictogramKey",
                table: "Activities",
                column: "PictogramKey");

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

        /// <summary>
        /// Rollback migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

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
                name: "UserResources");

            migrationBuilder.DropTable(
                name: "WeekDayColors");

            migrationBuilder.DropTable(
                name: "Weekdays");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

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
