using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations.MySQL
{
    public partial class AddWeekTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DepartmentKey",
                table: "Weeks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_DepartmentKey",
                table: "Weeks",
                column: "DepartmentKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Weeks_Departments_DepartmentKey",
                table: "Weeks",
                column: "DepartmentKey",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weeks_Departments_DepartmentKey",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_DepartmentKey",
                table: "Weeks");

            migrationBuilder.DropColumn(
                name: "DepartmentKey",
                table: "Weeks");
        }
    }
}
