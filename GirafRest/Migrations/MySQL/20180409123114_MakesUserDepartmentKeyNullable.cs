using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations.MySQL
{
    public partial class MakesUserDepartmentKeyNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentKey",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentKey",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentKey",
                table: "AspNetUsers",
                column: "DepartmentKey",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentKey",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentKey",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentKey",
                table: "AspNetUsers",
                column: "DepartmentKey",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
