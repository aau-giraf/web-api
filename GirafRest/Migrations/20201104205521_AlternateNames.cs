using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class AlternateNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlternateNames",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CitizenId = table.Column<string>(nullable: true),
                    PictogramId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternateNames", x => x.id);
                    table.ForeignKey(
                        name: "FK_AlternateNames_AspNetUsers_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlternateNames_Pictograms_PictogramId",
                        column: x => x.PictogramId,
                        principalTable: "Pictograms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNames_CitizenId",
                table: "AlternateNames",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNames_id",
                table: "AlternateNames",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlternateNames_PictogramId",
                table: "AlternateNames",
                column: "PictogramId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternateNames");
        }
    }
}
