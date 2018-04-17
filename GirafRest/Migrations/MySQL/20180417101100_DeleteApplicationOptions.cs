using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations.MySQL
{
    public partial class DeleteApplicationOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationOption");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationOption",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationName = table.Column<string>(nullable: false),
                    ApplicationPackage = table.Column<string>(nullable: false),
                    LauncherOptionsKey = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationOption_LauncherOptions_LauncherOptionsKey",
                        column: x => x.LauncherOptionsKey,
                        principalTable: "LauncherOptions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationOption_LauncherOptionsKey",
                table: "ApplicationOption",
                column: "LauncherOptionsKey");
        }
    }
}
