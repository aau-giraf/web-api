using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations.MySql
{
    public partial class AddedPictogramSound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFormat",
                table: "Frames");

            migrationBuilder.AddColumn<byte[]>(
                name: "Sound",
                table: "Frames",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sound",
                table: "Frames");

            migrationBuilder.AddColumn<int>(
                name: "ImageFormat",
                table: "Frames",
                nullable: true);
        }
    }
}
