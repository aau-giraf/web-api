using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class RemoveWeekThemeColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorThemeWeekSchedules",
                table: "Setting");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColorThemeWeekSchedules",
                table: "Setting",
                nullable: false,
                defaultValue: 0);
        }
    }
}
