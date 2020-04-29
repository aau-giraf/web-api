using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    /// <summary>
    /// Remove key for DepartmentResources
    /// </summary>
    public partial class RemoveUnusedFkinDepartmentResource : Migration
    {
        /// <summary>
        /// Run migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceKey",
                table: "DepartmentResources");
        }

        /// <summary>
        /// Rollback migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ResourceKey",
                table: "DepartmentResources",
                nullable: true);
        }
    }
}
