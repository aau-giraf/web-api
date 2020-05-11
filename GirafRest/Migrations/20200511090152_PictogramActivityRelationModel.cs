using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class PictogramActivityRelationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Pictograms_PictogramKey",
                table: "Activities");

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

            migrationBuilder.DropIndex(
                name: "IX_Activities_PictogramKey",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "WeekTemplates");

            migrationBuilder.DropColumn(
                name: "ThumbnailId",
                table: "Weeks");

            migrationBuilder.DropColumn(
                name: "PictogramKey",
                table: "Activities");

            migrationBuilder.CreateTable(
                name: "PictogramRelations",
                columns: table => new
                {
                    ActivityId = table.Column<long>(nullable: false),
                    PictogramId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictogramRelations", x => new { x.ActivityId, x.PictogramId });
                    table.ForeignKey(
                        name: "FK_PictogramRelations_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PictogramRelations_Pictograms_PictogramId",
                        column: x => x.PictogramId,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplates_ThumbnailKey",
                table: "WeekTemplates",
                column: "ThumbnailKey");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThumbnailKey",
                table: "Weeks",
                column: "ThumbnailKey");

            migrationBuilder.CreateIndex(
                name: "IX_PictogramRelations_PictogramId",
                table: "PictogramRelations",
                column: "PictogramId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weeks_Pictograms_ThumbnailKey",
                table: "Weeks");

            migrationBuilder.DropForeignKey(
                name: "FK_WeekTemplates_Pictograms_ThumbnailKey",
                table: "WeekTemplates");

            migrationBuilder.DropTable(
                name: "PictogramRelations");

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

            migrationBuilder.AddColumn<long>(
                name: "PictogramKey",
                table: "Activities",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WeekTemplates_ThumbnailId",
                table: "WeekTemplates",
                column: "ThumbnailId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_ThumbnailId",
                table: "Weeks",
                column: "ThumbnailId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PictogramKey",
                table: "Activities",
                column: "PictogramKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Pictograms_PictogramKey",
                table: "Activities",
                column: "PictogramKey",
                principalTable: "Pictograms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
