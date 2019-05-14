using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class RemoveImageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Pictograms");

            migrationBuilder.AddColumn<string>(
                name: "ImageHash",
                table: "Pictograms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageHash",
                table: "Pictograms");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Pictograms",
                nullable: true);
        }
    }
}
