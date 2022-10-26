using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirafRest.Migrations
{
    public partial class AddedShowOnlyActivitiesAndNrOfActivitiesToDisplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrOfActivitiesToDisplay",
                table: "Setting",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowOnlyActivities",
                table: "Setting",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrOfActivitiesToDisplay",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "ShowOnlyActivities",
                table: "Setting");
        }
    }
}
