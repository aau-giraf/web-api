using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class PictogramtextMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weeks_Pictograms_ThumbnailKey",
                table: "Weeks");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekTemplates_Pictograms_ThumbnailKey",
                table: "WeekTemplates");

            migrationBuilder.DropIndex(
                name: "IX_WeekTemplates_ThumbnailKey",
                table: "WeekTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_ThumbnailKey",
                table: "Weeks");

            migrationBuilder.AddColumn<long>(
                name: "ThumbnailId",
                table: "WeekTemplates",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThumbnailId",
                table: "Weeks",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PictogramText",
                table: "Setting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplates_ThumbnailId",
                table: "WeekTemplates",
                column: "ThumbnailId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThumbnailId",
                table: "Weeks",
                column: "ThumbnailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Weeks_Pictograms_ThumbnailId",
                table: "Weeks",
                column: "ThumbnailId",
                principalTable: "Pictograms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WeekTemplates_Pictograms_ThumbnailId",
                table: "WeekTemplates",
                column: "ThumbnailId",
                principalTable: "Pictograms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weeks_Pictograms_ThumbnailId",
                table: "Weeks");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekTemplates_Pictograms_ThumbnailId",
                table: "WeekTemplates");

            migrationBuilder.DropIndex(
                name: "IX_WeekTemplates_ThumbnailId",
                table: "WeekTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Weeks_ThumbnailId",
                table: "Weeks");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "WeekTemplates");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "Weeks");

            migrationBuilder.DropColumn(
                name: "PictogramText",
                table: "Setting");

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplates_ThumbnailKey",
                table: "WeekTemplates",
                column: "ThumbnailKey");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThumbnailKey",
                table: "Weeks",
                column: "ThumbnailKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Weeks_Pictograms_ThumbnailKey",
                table: "Weeks",
                column: "ThumbnailKey",
                principalTable: "Pictograms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeekTemplates_Pictograms_ThumbnailKey",
                table: "WeekTemplates",
                column: "ThumbnailKey",
                principalTable: "Pictograms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
