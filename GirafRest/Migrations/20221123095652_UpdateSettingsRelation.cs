using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirafRest.Migrations
{
    public partial class UpdateSettingsRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SettingsKey",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SettingsKey",
                table: "AspNetUsers",
                column: "SettingsKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SettingsKey",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SettingsKey",
                table: "AspNetUsers",
                column: "SettingsKey");
        }
    }
}
