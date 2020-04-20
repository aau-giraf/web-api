using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class AddedShowTimerBtnsVariable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowTimerBtns",
                table: "Setting",
                nullable: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowTimerBtns",
                table: "Setting"
            );
        }
    }
}
