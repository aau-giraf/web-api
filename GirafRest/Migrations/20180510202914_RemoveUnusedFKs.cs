using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    public partial class RemoveUnusedFKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceKey",
                table: "UserResources");

            migrationBuilder.DropColumn(
                name: "ResourceKey",
                table: "Activities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ResourceKey",
                table: "UserResources",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResourceKey",
                table: "Activities",
                nullable: true);
        }
    }
}
