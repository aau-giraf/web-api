using Microsoft.EntityFrameworkCore.Migrations;

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
