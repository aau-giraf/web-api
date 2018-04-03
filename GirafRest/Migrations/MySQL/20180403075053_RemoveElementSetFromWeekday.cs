using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations.MySQL
{
    public partial class RemoveElementSetFromWeekday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElementsSet",
                table: "Weekdays");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ElementsSet",
                table: "Weekdays",
                nullable: false,
                defaultValue: true);
        }
    }
}
