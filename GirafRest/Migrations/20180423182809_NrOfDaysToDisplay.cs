using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class NrOfDaysToDisplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "timerSeconds",
                table: "LauncherOptions",
                newName: "TimerSeconds");

            migrationBuilder.RenameColumn(
                name: "theme",
                table: "LauncherOptions",
                newName: "Theme");

            migrationBuilder.RenameColumn(
                name: "orientation",
                table: "LauncherOptions",
                newName: "Orientation");

            migrationBuilder.RenameColumn(
                name: "displayLauncherAnimations",
                table: "LauncherOptions",
                newName: "DisplayLauncherAnimations");

            migrationBuilder.RenameColumn(
                name: "defaultTimer",
                table: "LauncherOptions",
                newName: "DefaultTimer");

            migrationBuilder.RenameColumn(
                name: "checkResourceAppearence",
                table: "LauncherOptions",
                newName: "CheckResourceAppearence");

            migrationBuilder.RenameColumn(
                name: "appGridSizeRows",
                table: "LauncherOptions",
                newName: "AppGridSizeRows");

            migrationBuilder.RenameColumn(
                name: "appGridSizeColumns",
                table: "LauncherOptions",
                newName: "AppGridSizeColumns");

            migrationBuilder.RenameColumn(
                name: "activitiesCount",
                table: "LauncherOptions",
                newName: "ActivitiesCount");

            migrationBuilder.AddColumn<int>(
                name: "NrOfDaysToDisplay",
                table: "LauncherOptions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrOfDaysToDisplay",
                table: "LauncherOptions");

            migrationBuilder.RenameColumn(
                name: "TimerSeconds",
                table: "LauncherOptions",
                newName: "timerSeconds");

            migrationBuilder.RenameColumn(
                name: "Theme",
                table: "LauncherOptions",
                newName: "theme");

            migrationBuilder.RenameColumn(
                name: "Orientation",
                table: "LauncherOptions",
                newName: "orientation");

            migrationBuilder.RenameColumn(
                name: "DisplayLauncherAnimations",
                table: "LauncherOptions",
                newName: "displayLauncherAnimations");

            migrationBuilder.RenameColumn(
                name: "DefaultTimer",
                table: "LauncherOptions",
                newName: "defaultTimer");

            migrationBuilder.RenameColumn(
                name: "CheckResourceAppearence",
                table: "LauncherOptions",
                newName: "checkResourceAppearence");

            migrationBuilder.RenameColumn(
                name: "AppGridSizeRows",
                table: "LauncherOptions",
                newName: "appGridSizeRows");

            migrationBuilder.RenameColumn(
                name: "AppGridSizeColumns",
                table: "LauncherOptions",
                newName: "appGridSizeColumns");

            migrationBuilder.RenameColumn(
                name: "ActivitiesCount",
                table: "LauncherOptions",
                newName: "activitiesCount");
        }
    }
}
