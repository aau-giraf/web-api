using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class MakeGirafIdUnWeekRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Weeks WHERE GirafUserId is NULL");
            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: false,
                maxLength: 127,
                type: "varchar(127)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GirafUserId",
                table: "Weeks",
                nullable: true,
                maxLength: 127,
                type: "varchar(127)"
            );
        }
    }
}
