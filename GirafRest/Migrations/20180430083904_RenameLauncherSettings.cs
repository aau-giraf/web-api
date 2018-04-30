using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class RenameLauncherSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LauncherOptions_SettingsKey",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "LauncherOptions");

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivitiesCount = table.Column<int>(nullable: true),
                    CancelMark = table.Column<int>(nullable: false),
                    ColorThemeWeekSchedules = table.Column<int>(nullable: false),
                    CompleteMark = table.Column<int>(nullable: false),
                    DefaultTimer = table.Column<int>(nullable: false),
                    GreyScale = table.Column<bool>(nullable: false),
                    NrOfDaysToDisplay = table.Column<int>(nullable: true),
                    Orientation = table.Column<int>(nullable: false),
                    Theme = table.Column<int>(nullable: false),
                    TimerSeconds = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Key);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Setting_SettingsKey",
                table: "AspNetUsers",
                column: "SettingsKey",
                principalTable: "Setting",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Setting_SettingsKey",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.CreateTable(
                name: "LauncherOptions",
                columns: table => new
                {
                    Key = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivitiesCount = table.Column<int>(nullable: true),
                    AppGridSizeColumns = table.Column<int>(nullable: true),
                    AppGridSizeRows = table.Column<int>(nullable: true),
                    CancelMark = table.Column<int>(nullable: false),
                    CompleteMark = table.Column<int>(nullable: false),
                    DefaultTimer = table.Column<int>(nullable: false),
                    DisplayLauncherAnimations = table.Column<bool>(nullable: false),
                    NrOfDaysToDisplay = table.Column<int>(nullable: true),
                    Orientation = table.Column<int>(nullable: false),
                    Theme = table.Column<int>(nullable: false),
                    TimerSeconds = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LauncherOptions", x => x.Key);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LauncherOptions_SettingsKey",
                table: "AspNetUsers",
                column: "SettingsKey",
                principalTable: "LauncherOptions",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
