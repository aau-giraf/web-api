using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafWebApi.Migrations
{
    public partial class InitialIdentityServerMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frame_Departments_Department_Key",
                table: "Frame");

            migrationBuilder.DropForeignKey(
                name: "FK_Frame_Departments_DepartmentKey",
                table: "Frame");

            migrationBuilder.DropForeignKey(
                name: "FK_Frame_Image_ImageKey",
                table: "Frame");

            migrationBuilder.DropForeignKey(
                name: "FK_Frame_Frame_ThumbnailKey",
                table: "Frame");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_Department_Key",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Image_IconKey",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Image",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Frame",
                table: "Frame");

            migrationBuilder.RenameTable(
                name: "Image",
                newName: "Images");

            migrationBuilder.RenameTable(
                name: "Frame",
                newName: "Frames");

            migrationBuilder.RenameColumn(
                name: "IconKey",
                table: "AspNetUsers",
                newName: "DepartmentKey");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_IconKey",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DepartmentKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frame_ThumbnailKey",
                table: "Frames",
                newName: "IX_Frames_ThumbnailKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frame_ImageKey",
                table: "Frames",
                newName: "IX_Frames_ImageKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frame_DepartmentKey",
                table: "Frames",
                newName: "IX_Frames_DepartmentKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frame_Department_Key",
                table: "Frames",
                newName: "IX_Frames_Department_Key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Frames",
                table: "Frames",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Departments_Department_Key",
                table: "Frames",
                column: "Department_Key",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Departments_DepartmentKey",
                table: "Frames",
                column: "DepartmentKey",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Images_ImageKey",
                table: "Frames",
                column: "ImageKey",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Frames_ThumbnailKey",
                table: "Frames",
                column: "ThumbnailKey",
                principalTable: "Frames",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentKey",
                table: "AspNetUsers",
                column: "DepartmentKey",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Images_Department_Key",
                table: "AspNetUsers",
                column: "Department_Key",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Departments_Department_Key",
                table: "Frames");

            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Departments_DepartmentKey",
                table: "Frames");

            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Images_ImageKey",
                table: "Frames");

            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Frames_ThumbnailKey",
                table: "Frames");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentKey",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Images_Department_Key",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Frames",
                table: "Frames");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "Image");

            migrationBuilder.RenameTable(
                name: "Frames",
                newName: "Frame");

            migrationBuilder.RenameColumn(
                name: "DepartmentKey",
                table: "AspNetUsers",
                newName: "IconKey");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DepartmentKey",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_IconKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frames_ThumbnailKey",
                table: "Frame",
                newName: "IX_Frame_ThumbnailKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frames_ImageKey",
                table: "Frame",
                newName: "IX_Frame_ImageKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frames_DepartmentKey",
                table: "Frame",
                newName: "IX_Frame_DepartmentKey");

            migrationBuilder.RenameIndex(
                name: "IX_Frames_Department_Key",
                table: "Frame",
                newName: "IX_Frame_Department_Key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Image",
                table: "Image",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Frame",
                table: "Frame",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Frame_Departments_Department_Key",
                table: "Frame",
                column: "Department_Key",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Frame_Departments_DepartmentKey",
                table: "Frame",
                column: "DepartmentKey",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Frame_Image_ImageKey",
                table: "Frame",
                column: "ImageKey",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Frame_Frame_ThumbnailKey",
                table: "Frame",
                column: "ThumbnailKey",
                principalTable: "Frame",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_Department_Key",
                table: "AspNetUsers",
                column: "Department_Key",
                principalTable: "Departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Image_IconKey",
                table: "AspNetUsers",
                column: "IconKey",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
