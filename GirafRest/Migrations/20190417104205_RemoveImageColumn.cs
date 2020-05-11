using Microsoft.EntityFrameworkCore.Migrations;

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
