using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class CascadingDeleteRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Timers_TimerKey",
                table: "Activities");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Timers_TimerKey",
                table: "Activities",
                column: "TimerKey",
                principalTable: "Timers",
                principalColumn: "Key",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Timers_TimerKey",
                table: "Activities");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Timers_TimerKey",
                table: "Activities",
                column: "TimerKey",
                principalTable: "Timers",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
