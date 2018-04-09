using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations.MySQL
{
    public partial class AddIndexing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Frames",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_id",
                table: "Weeks",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weekdays_id",
                table: "Weekdays",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Frames_id_Title",
                table: "Frames",
                columns: new[] { "id", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Id_UserName",
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Weeks_id",
                table: "Weeks");

            migrationBuilder.DropIndex(
                name: "IX_Weekdays_id",
                table: "Weekdays");

            migrationBuilder.DropIndex(
                name: "IX_Frames_id_Title",
                table: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Id_UserName",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Frames",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
