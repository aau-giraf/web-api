using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations.MySQL
{
    public partial class updatelaunchersettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseGrayscale",
                table: "LauncherOptions");

            migrationBuilder.AddColumn<int>(
                name: "activitiesCount",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "checkResourceAppearence",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "defaultTimer",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "orientation",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "theme",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "timerSeconds",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activitiesCount",
                table: "LauncherOptions");

            migrationBuilder.DropColumn(
                name: "checkResourceAppearence",
                table: "LauncherOptions");

            migrationBuilder.DropColumn(
                name: "defaultTimer",
                table: "LauncherOptions");

            migrationBuilder.DropColumn(
                name: "orientation",
                table: "LauncherOptions");

            migrationBuilder.DropColumn(
                name: "theme",
                table: "LauncherOptions");

            migrationBuilder.DropColumn(
                name: "timerSeconds",
                table: "LauncherOptions");

            migrationBuilder.AddColumn<bool>(
                name: "UseGrayscale",
                table: "LauncherOptions",
                nullable: false,
                defaultValue: false);
        }
    }
}
