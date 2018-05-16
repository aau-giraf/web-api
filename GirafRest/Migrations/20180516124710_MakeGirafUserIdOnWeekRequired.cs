using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class MakeGirafUserIdOnWeekRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
