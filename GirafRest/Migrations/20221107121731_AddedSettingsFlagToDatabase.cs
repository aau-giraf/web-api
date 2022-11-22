using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirafRest.Migrations
{
    public partial class AddedSettingsFlagToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowSettingsForCitizen",
                table: "Setting",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowSettingsForCitizen",
                table: "Setting");
        }
    }
}
