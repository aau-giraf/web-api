using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafWebApi.Migrations
{
    public partial class Giraf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Departments_Department_Key",
                table: "Frames");

            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Images_ImageKey",
                table: "Frames");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Images_Department_Key",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Department_Key",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Frames_Department_Key",
                table: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Frames_ImageKey",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "Department_Key",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Department_Key",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "ImageKey",
                table: "Frames");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastEdit",
                table: "Frames",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AccessLevel",
                table: "Frames",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GirafUserId",
                table: "Frames",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Frames",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Frames_GirafUserId",
                table: "Frames",
                column: "GirafUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_AspNetUsers_GirafUserId",
                table: "Frames",
                column: "GirafUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_AspNetUsers_GirafUserId",
                table: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Frames_GirafUserId",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lastEdit",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "AccessLevel",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "GirafUserId",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Frames");

            migrationBuilder.AddColumn<long>(
                name: "Department_Key",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Department_Key",
                table: "Frames",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "owner_id",
                table: "Frames",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ImageKey",
                table: "Frames",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Department_Key",
                table: "AspNetUsers",
                column: "Department_Key");

            migrationBuilder.CreateIndex(
                name: "IX_Frames_Department_Key",
                table: "Frames",
                column: "Department_Key");

            migrationBuilder.CreateIndex(
                name: "IX_Frames_ImageKey",
                table: "Frames",
                column: "ImageKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Departments_Department_Key",
                table: "Frames",
                column: "Department_Key",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Images_ImageKey",
                table: "Frames",
                column: "ImageKey",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Images_Department_Key",
                table: "AspNetUsers",
                column: "Department_Key",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
