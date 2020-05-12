using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    /// <summary>
    /// Removes Image column from Pictograms, and adds a ImageHash.
    /// </summary>
    public partial class RemoveImageColumn : Migration
    {
        /// <summary>
        /// Run migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) {
                throw new System.ArgumentNullException(migrationBuilder + " is null");
            }
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Pictograms");

            migrationBuilder.AddColumn<string>(
                name: "ImageHash",
                table: "Pictograms",
                nullable: true);
        }

        /// <summary>
        /// Rollback migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) {
                throw new System.ArgumentNullException(migrationBuilder + " is null");
            }
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
