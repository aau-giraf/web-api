using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GirafRest.Migrations
{
    /// <summary>
    /// Remove unused FK's
    /// </summary>
    public partial class RemoveUnusedFKs : Migration
    {
        /// <summary>
        /// Run migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) 
                throw new System.ArgumentNullException(nameof(migrationBuilder) + new string(" is null"));
            
            migrationBuilder.DropColumn(
                name: "ResourceKey",
                table: "UserResources");

            migrationBuilder.DropColumn(
                name: "ResourceKey",
                table: "Activities");
        }

        /// <summary>
        /// Rollback migration here
        /// </summary>
        /// <param name="migrationBuilder">Which MigrationBuilder to use</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null) 
                throw new System.ArgumentNullException(nameof(migrationBuilder) + new string(" is null"));
            
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
