using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class LockTimerControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LockTimerControl",
                table: "Setting",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockTimerControl",
                table: "Setting");
        }
    }
}
